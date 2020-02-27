using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreProfilerUnitTest.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (value.Trim().Length <= maxLength)
            {
                return value.Trim();
            }

            return value.Trim().Substring(0, maxLength);
        }

        public static DbString ToNVarchar(this string value)
        {
            return new DbString()
            {
                Value = value,
            };
        }
    }
}