using System;
using System.Linq;

using Microsoft.Extensions.Configuration;

namespace CoreProfilerUnitTest.RepositoryTests.TestUtilities
{
    public class TestSettingProvider
    {
        private static TestSettings Settings { get; set; }

        /// <summary>
        /// 取得測試設定內容.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        internal static TestSettings GetSettings(string fileName = "TestSettings.json")
        {
            if (Settings != (object)null)
            {
                return Settings;
            }

            var testSettings = new TestSettings();

            var configuration = new ConfigurationBuilder().AddJsonFile(fileName).Build();
            configuration.Bind(testSettings);

            Settings = testSettings;
            return Settings;
        }

        /// <summary>
        /// 取得測試資料庫的類別，localdb or docker.
        /// </summary>
        /// <returns>System.String.</returns>
        internal static string GetTestDatabaseType()
        {
            var testSettings = GetSettings();

            var types = new[] { "localdb", "docker" };

            var databaseType = string.IsNullOrWhiteSpace(testSettings.DatabaseType)
                ? "localdb"
                : types.Contains(testSettings.DatabaseType.ToLower()).Equals(false)
                    ? "localdb"
                    : testSettings.DatabaseType.ToLower();

            return databaseType;
        }

        /// <summary>
        /// 取得測試資料庫所指定使用的 docker image 名稱與 ContainerReadyMessage.
        /// </summary>
        /// <returns>System.String.</returns>
        internal static ContainerSetting GetContainerSetting()
        {
            var testSettings = GetSettings();

            var dockerType = string.IsNullOrWhiteSpace(testSettings.DockerType)
                ? string.Empty
                : testSettings.DockerType.ToLower();

            var currentDockerOSType = DockerSupports.GetDockerVersionOsType();
            if (dockerType.Equals(currentDockerOSType, StringComparison.OrdinalIgnoreCase).Equals(false))
            {
                dockerType = currentDockerOSType.ToLower();
            }

            switch (dockerType)
            {
                case "linux":
                    return testSettings.LinuxDatabase;

                case "windows":
                    return testSettings.WindowsDatabase;

                default:
                    return new ContainerSetting();
            }
        }
    }

    internal class TestSettings
    {
        /// <summary>
        /// 測試資料庫類型：localdb or docker.
        /// </summary>
        public string DatabaseType { get; set; }

        /// <summary>
        /// Docker 類型：linux or windows.
        /// </summary>
        public string DockerType { get; set; }

        /// <summary>
        /// Container Label.
        /// </summary>
        public string ContainerLabel { get; set; }

        public Linuxdatabase LinuxDatabase { get; set; }

        public Windowsdatabase WindowsDatabase { get; set; }
    }

    internal class ContainerSetting
    {
        public string DatabaseImage { get; set; }

        public string ContainerReadyMessage { get; set; }
    }

    internal class Linuxdatabase : ContainerSetting
    {
    }

    internal class Windowsdatabase : ContainerSetting
    {
    }
}