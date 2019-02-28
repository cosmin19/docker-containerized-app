using System;
using System.Linq;

namespace Enviroself.Infrastructure.Utilities
{
    public static class StringsProperties {
        public static T TrimProperties<T>(this T input)
        {
            var stringProperties = input.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            foreach (var stringProperty in stringProperties)
            {
                string currentValue = (string)stringProperty.GetValue(input, null);
                if (currentValue != null)
                    stringProperty.SetValue(input, currentValue.Trim(), null);
            }
            return input;
        }

        public static String GetHumanReadableFileSize(this long input)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (input >= 1024 && order < sizes.Length - 1)
            {
                order++;
                input = input / 1024;
            }

            return String.Format("{0:0.##} {1}", input, sizes[order]);
        }

        public static String GetHumanReadableFileSize(this decimal input)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (input >= 1024 && order < sizes.Length - 1)
            {
                order++;
                input = input / 1024;
            }

            return String.Format("{0:0.##} {1}", input, sizes[order]);
        }

    }
}
