using System.Diagnostics;
using System.Globalization;

namespace CrossCert.Converters
{
    /// <summary>
    /// Converts an integer count to an inverted boolean value.
    /// Returns true if the count is 0 or less. Useful for showing an empty state message.
    /// </summary>
    public class CountToInverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count <= 0;
            }

            // Optional: Log non-integer input for debugging
            if (value != null)
            {
                Debug.WriteLine($"[Converter Error] Expected int for CountToInverseBoolConverter, got {value.GetType().Name}");
            }

            // Default to true for null or incorrect values (assuming an empty state)
            return true;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // One-way conversion only
            throw new NotImplementedException("CountToInverseBoolConverter is a one-way converter.");
        }
    }
}
