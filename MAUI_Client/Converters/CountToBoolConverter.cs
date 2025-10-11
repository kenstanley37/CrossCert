using System.Diagnostics;
using System.Globalization;

namespace CrossCert.Converters
{
    /// <summary>
    /// Converts an integer count to a boolean value.
    /// Returns true if the count is greater than 0. Useful for showing controls when a list has items.
    /// </summary>
    public class CountToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count > 0;
            }

            // Optional: Log non-integer input for debugging
            if (value != null)
            {
                Debug.WriteLine($"[Converter Error] Expected int for CountToBoolConverter, got {value.GetType().Name}");
            }

            // Default to false for null or incorrect values
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // One-way conversion only
            throw new NotImplementedException("CountToBoolConverter is a one-way converter.");
        }
    }
}
