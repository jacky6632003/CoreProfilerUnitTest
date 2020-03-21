using CliWrap;
using CliWrap.Buffered;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CoreProfilerUnitTest.RepositoryTests.TestUtilities
{
    /// <summary>
    /// class DockerSupports
    /// </summary>
    public class DockerSupports
    {
        private static string _defaultContainerLabel;

        internal static string DefaultContainerLabel
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultContainerLabel))
                {
                    _defaultContainerLabel = typeof(DockerSupports).Assembly.GetName().Name.ToLower();
                }

                return _defaultContainerLabel;
            }
            set => _defaultContainerLabel = value;
        }

        internal static string ContainerId { get; set; }

        private static int _port;

        internal static int Port
        {
            get
            {
                if (_port.Equals(0).Equals(false))
                {
                    return _port;
                }

                var rnd = new Random();
                var result = rnd.Next(49152, 65535);
                _port = result;
                return _port;
            }
        }

        private static string _currentOsType;

        internal static string CurrentOsType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_currentOsType))
                {
                    _currentOsType = "windows";
                }

                return _currentOsType;
            }
            set => _currentOsType = value;
        }

        /// <summary>
        /// Gets the docker version OS Type.
        /// </summary>
        /// <returns>System.String.</returns>
        internal static string GetDockerVersionOsType()
        {
            var executeResult = Cli.Wrap("docker")
                                   .WithArguments("version")
                                   .ExecuteBufferedAsync()
                                   .Select(x => x.StandardOutput)
                                   .GetAwaiter().GetResult();

            const string windowsOsType = "windows/amd64";
            const string linuxOsType = "linux/amd64";

            var windowsCount = 0;
            var linuxCount = 0;

            using (var reader = new StringReader(executeResult))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("OS/Arch").Equals(false))
                    {
                        continue;
                    }

                    Console.WriteLine(line);

                    var osArch = line.Split(':')[1].Trim();

                    if (osArch.Equals(windowsOsType, StringComparison.OrdinalIgnoreCase))
                    {
                        windowsCount += 1;
                    }
                    else if (osArch.Equals(linuxOsType, StringComparison.OrdinalIgnoreCase))
                    {
                        linuxCount += 1;
                    }
                }
            }

            var result = linuxCount.Equals(2)
                ? "linux"
                : windowsCount.Equals(1) && linuxCount.Equals(1)
                    ? "linux"
                    : "windows";

            CurrentOsType = result;

            return result;
        }

        /// <summary>
        /// Checks the image.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        internal static bool CheckImage(string imageName)
        {
            // 檢查指定的 image 是否存在
            var executeResult = Cli.Wrap("docker")
                                   .WithArguments($"images {imageName}")
                                   .ExecuteBufferedAsync()
                                   .Select(x => x.StandardOutput)
                                   .GetAwaiter().GetResult();

            using (var reader = new StringReader(executeResult))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(imageName).Equals(false))
                    {
                        continue;
                    }

                    Console.WriteLine(line);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="containerReadyMessage">The container ready message.</param>
        /// <param name="containerLabel">The container label.</param>
        /// <returns>
        /// System.Boolean.
        /// </returns>
        internal static bool CreateContainer(string imageName, string containerReadyMessage, string containerLabel = "")
        {
            containerLabel = string.IsNullOrWhiteSpace(containerLabel)
                ? DefaultContainerLabel
                : containerLabel.ToLower();

            // 建立前要先把之前測試所建立的 container 給移除
            StopContainer(containerLabel);

            // 使用指定的 image 建立 docker container
            var executeArguments = $"run --rm -d -e SA_PASSWORD=1q2w3e4r5t_ -e ACCEPT_EULA=Y --label={containerLabel} -ti -p {Port}:1433 {imageName}";
            Console.WriteLine($"docker {executeArguments}");

            var executeResult = Cli.Wrap("docker")
                                   .WithArguments(executeArguments)
                                   .ExecuteBufferedAsync()
                                   .Select(x => x.StandardOutput)
                                   .GetAwaiter().GetResult();

            var lineCount = 0;

            using (var reader = new StringReader(executeResult))
            {
                // 取得 Container ID，如有錯誤訊息則中斷測試

                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (lineCount.Equals(0))
                    {
                        Console.WriteLine(line);
                        ContainerId = line.Substring(0, 12);
                        lineCount += 1;
                    }
                    else
                    {
                        Assert.Fail(line);
                    }
                }
            }

            // 執行 docker logs 指令，查看 container 裡的 sql-server 服務是否已經準備完成
            const int retryTimes = 120;
            var ready = false;

            for (var i = 0; i < retryTimes; i++)
            {
                var output = Cli.Wrap("docker")
                                .WithArguments($"logs {ContainerId}")
                                .ExecuteBufferedAsync()
                                .Select(x => x.StandardOutput)
                                .GetAwaiter().GetResult();

                var logs = output;

                if (logs.Contains(containerReadyMessage))
                {
                    ready = true;
                }

                if (ready.Equals(false))
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.WriteLine($"wait {i} times");
                    break;
                }
            }

            return ready;
        }

        /// <summary>
        /// 取得 Container 內部的 IP (for Windows Container).
        /// </summary>
        /// <param name="containerId">The container id.</param>
        /// <returns>System.String.</returns>
        internal static string GetContainerIp(string containerId)
        {
            var executeResult = Cli.Wrap("docker")
                                   .WithArguments($"exec {containerId} ipconfig")
                                   .ExecuteBufferedAsync()
                                   .Select(x => x.StandardOutput)
                                   .GetAwaiter().GetResult();

            var containerInsideIp = string.Empty;

            using (var reader = new StringReader(executeResult))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("IPv4 Address").Equals(false))
                    {
                        continue;
                    }

                    containerInsideIp = line.Split(':')[1].Trim();
                    break;
                }
            }

            var databaseIp = string.IsNullOrWhiteSpace(containerInsideIp) ? "" : containerInsideIp;
            return databaseIp;
        }

        /// <summary>
        /// Stop the container.
        /// </summary>
        /// <param name="containerLabel">The container label.</param>
        internal static void StopContainer(string containerLabel = "")
        {
            containerLabel = string.IsNullOrWhiteSpace(containerLabel)
                ? DefaultContainerLabel
                : containerLabel.ToLower();

            // 檢查指定的 container 是否存在
            var executeResult = Cli.Wrap("docker")
                                   .WithValidation(CommandResultValidation.None)
                                   .WithArguments($"ps --quiet --filter label={containerLabel}")
                                   .ExecuteBufferedAsync()
                                   .Select(x => x.StandardOutput)
                                   .GetAwaiter().GetResult();

            if (string.IsNullOrWhiteSpace(executeResult).Equals(true))
            {
                return;
            }

            // 如果 container 存在則移除測試資料庫的 container
            executeResult = Cli.Wrap("docker")
                               .WithArguments($"stop {executeResult.Trim().Replace("\r\n", " ")}")
                               .ExecuteBufferedAsync()
                               .Select(x => x.StandardOutput)
                               .GetAwaiter().GetResult();

            using (var reader = new StringReader(executeResult))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}