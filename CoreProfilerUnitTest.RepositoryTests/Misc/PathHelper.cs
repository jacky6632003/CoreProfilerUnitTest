using System;
using System.Collections.Generic;
using System.Text;

namespace CoreProfilerUnitTest.RepositoryTests.Misc
{
    public class PathHelper
    {
        /// <summary>
        /// Replaces the path characters.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static string ReplacePathCharacters(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return string.Empty;
            }

            if (OperatingSystem.IsLinux())
            {
                filePath = filePath.Replace(@"\", @"/");
            }

            return filePath;
        }
    }
}