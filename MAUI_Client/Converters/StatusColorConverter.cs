using CrossCert.Models;
using System.Globalization;

// The namespace must match the file's new location in the project structure (e.g., /Converters folder)
namespace CrossCert.Converters
{
    /// <summary>
    /// Converts a Domain object's StatusText property into a visual color indicator.
    /// </summary>
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Domain domain)
            {
                // *** FIX: The converter now relies on the StatusText property already calculated in the Domain model or its associated Certificate. ***
                // It no longer attempts to call the removed static method 'MainPageViewModel.GetDomainStatus(domain)'.
                string status = domain.StatusText;

                // Use the appropriate color for the status
                if (status.Contains("Active"))
                {
                    return Color.FromArgb("#4CAF50"); // Green (Success)
                }
                else if (status.Contains("EXPIRED"))
                {
                    return Color.FromArgb("#F44336"); // Red (Danger)
                }
                else if (status.Contains("Expiring Soon"))
                {
                    return Color.FromArgb("#FFC107"); // Amber/Yellow (Warning)
                }
                else // Pending or other non-active states
                {
                    return Color.FromArgb("#9E9E9E"); // Gray/Pending
                }
            }

            // Fallback for non-domain objects or null
            return Color.FromArgb("#000000");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
