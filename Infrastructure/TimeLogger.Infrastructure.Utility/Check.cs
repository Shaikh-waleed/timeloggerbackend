using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Utility
{
    public static class Check
    {

        public static bool NotNull<T>(T value) where T : class
        {
            return value != null;
        }

        public static bool NotNull<T>(T? value) where T : struct
        {
            return value != null;
        }
        public static bool Null<T>(T value) where T : class
        {
            return value == null;
        }

        public static bool Null<T>(T? value) where T : struct
        {
            return value == null;
        }

        public static bool NotEmpty(string value)
        {
            return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
        }
        public static bool NotZero(int value)
        {
            return value != 0;
        }
        public static bool Zero(int value)
        {
            return value == 0;
        }
        public static bool Equals<T>(T value1, T value2) where T : struct
        {
            return EqualityComparer<T>.Default.Equals(value1, value2);
        }
        public static bool NotEquals<T>(T value1, T value2) where T : struct
        {
            return !EqualityComparer<T>.Default.Equals(value1, value2);
        }
    }
}
