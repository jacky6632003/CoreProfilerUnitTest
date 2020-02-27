using CoreProfilerUnitTest.RepositoryTests.Misc;
using CoreProfilerUnitTest.RepositoryTests.TestUtilities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreProfilerUnitTest.RepositoryTests
{
    [TestClass]
    public class TestHook
    {
        internal static TestSettings CurrentTestSettings { get; set; }

        internal static string DatabaseIp { get; set; }

        internal static string SampleDbConnectionString => DatabaseType.Equals("docker", StringComparison.OrdinalIgnoreCase)
            ? string.Format(TestDbConnections.Container.Database, DatabaseIp, DatabaseName) //$"Data Source={DatabaseIp};Initial Catalog={DatabaseName};Persist Security Info=True;User ID=sa;Password=1q2w3e4r5t_;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;TrustServerCertificate=True"
            : string.Format(TestDbConnections.LocalDb.Database, DatabaseName);

        private static string DatabaseType { get; set; }
        private static string DatabaseName => "SampleDB";

        [AssemblyInitialize]
        [Timeout(300)]
        public static void AssemblyInitialize(TestContext context)
        {
            CurrentTestSettings = TestSettingProvider.GetSettings();
            DatabaseType = TestSettingProvider.GetTestDatabaseType();

            if (DatabaseType.Equals("docker", StringComparison.OrdinalIgnoreCase))
            {
                // 取得 TestSettings 裡的 Docker 設定
                var containerSetting = TestSettingProvider.GetContainerSetting();
                var containerLabel = CurrentTestSettings.ContainerLabel;

                CreateDockerContainer(containerSetting, containerLabel);
            }
            else
            {
                TestLocalDbProcess.CreateDatabase(TestDbConnections.LocalDb.Master, DatabaseName);
            }

            // FluentAssertions 設定 : 日期時間使用接近比對的方式，而非完全一致的比對
            AssertionOptions.AssertEquivalencyUsing(options =>
            {
                options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation))
                       .WhenTypeIs<DateTime>();

                return options;
            });
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (DatabaseType.Equals("docker", StringComparison.OrdinalIgnoreCase))
            {
                var containerLabel = CurrentTestSettings.ContainerLabel;
                DockerSupports.StopContainer(containerLabel);
            }
            else
            {
                TestLocalDbProcess.DestroyDatabase(TestDbConnections.LocalDb.Master, DatabaseName);
            }
        }

        /// <summary>
        /// Creates the docker container.
        /// </summary>
        /// <param name="containerSetting">The container setting.</param>
        /// <param name="containerLabel">The container label.</param>
        private static void CreateDockerContainer(ContainerSetting containerSetting, string containerLabel)
        {
            var imageName = containerSetting.DatabaseImage;
            var containerReadyMessage = containerSetting.ContainerReadyMessage;

            // 確認指定的 docker image 是否存在
            var imageExists = DockerSupports.CheckImage(imageName);
            if (imageExists.Equals(false))
            {
                Assert.Fail($"docker image {imageName} not exists.");
            }

            // 以指定的 docker image 建立測試用的 container
            var isReady = DockerSupports.CreateContainer(imageName, containerReadyMessage, containerLabel);
            if (isReady.Equals(false))
            {
                Assert.Fail("create docker container failure.");
            }

            // 取得 container 的 ip
            DatabaseIp = imageName.Contains("windows")
                ? DockerSupports.GetContainerIp(DockerSupports.ContainerId)
                : $"127.0.0.1,{DockerSupports.Port}";

            if (string.IsNullOrWhiteSpace(DatabaseIp))
            {
                Assert.Fail("can not get docker container inside ip.");
            }

            // 在 container 裡的 SQL Server 建立測試用 Database
            var connectionString = string.Format(TestDbConnections.Container.Master, DatabaseIp);
            DatabaseCommands.CreateDatabase(connectionString, DatabaseName);
        }
    }
}