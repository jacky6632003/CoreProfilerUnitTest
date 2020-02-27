using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CoreProfilerUnitTest.RepositoryTests.Misc
{
    public static class OperatingSystem
    {
        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsMacOS()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public static bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }
    }
}