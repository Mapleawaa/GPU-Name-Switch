using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GpuSpoofer.Converters;

public class VendorColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var vendor = value as string;
        var hex = BrandColors.GetHex(vendor ?? "");
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
