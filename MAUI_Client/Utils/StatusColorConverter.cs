using CrossCert.Models;
using CrossCert.ViewModels;
using System.Globalization;

namespace CrossCert.Utils
{
    /// <summary>
    /// Converts a Domain object's status into a visual color indicator.
    /// </summary>
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Domain domain)
            {
                // Get the raw status text
                string status = MainPageViewModel.GetDomainStatus(domain);

                // Use the appropriate resource key for color-coding
                if (status.Contains("Active"))
                {
                    return Color.FromArgb("#4CAF50"); // Green
                }
                else if (status.Contains("Expired"))
                {
                    return Color.FromArgb("#F44336"); // Red
                }
                else if (status.Contains("Expiring Soon"))
                {
                    return Color.FromArgb("#FFC107"); // Amber/Yellow
                }
                else // Pending Verification / No Certificate
                {
                    return Color.FromArgb("#9E9E9E"); // Gray/Pending
                }
            }

            return Color.FromArgb("#000000"); // Default Black
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
