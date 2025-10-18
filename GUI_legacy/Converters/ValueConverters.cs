using CrossCert.Models;
using System.Globalization;

namespace CrossCert.ValueConverters
{
    /// <summary>
    /// Converts a Certificate object to a status text string (e.g., "OK", "Expired", "Pending").
    /// </summary>
    public class CertStatusToTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var certificate = value as Certificate;

            if (certificate == null)
            {
                return "Pending Config";
            }

            // Placeholder logic: 
            if (certificate.ExpiryDate < DateTime.Now)
            {
                return "Expired";
            }

            if (certificate.ExpiryDate < DateTime.Now.AddDays(30))
            {
                return "Renewal Due";
            }

            return "Active";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts a Certificate object to a corresponding color for a status badge.
    /// </summary>
    public class CertStatusToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var certificate = value as Certificate;

            if (certificate == null)
            {
                // Pending Config (Gray/Warning)
                return Color.FromArgb("#FFA500"); // Orange
            }

            // Placeholder Logic:
            if (certificate.ExpiryDate < DateTime.Now)
            {
                // Expired (Danger)
                return Color.FromArgb("#FF0000"); // Red
            }

            if (certificate.ExpiryDate < DateTime.Now.AddDays(30))
            {
                // Renewal Due (Warning)
                return Color.FromArgb("#FFFF00"); // Yellow
            }

            // Active (Success)
            return Color.FromArgb("#00FF00"); // Green
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}