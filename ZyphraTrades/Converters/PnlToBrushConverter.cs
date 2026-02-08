using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ZyphraTrades.Presentation.Converters;

public class PnlToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush Positive = new(Color.FromRgb(0x22, 0xC5, 0x5E));
    private static readonly SolidColorBrush Negative = new(Color.FromRgb(0xE1, 0x1D, 0x48));
    private static readonly SolidColorBrush Neutral = new(Color.FromRgb(0xA8, 0xA8, 0xB3));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal d)
            return d > 0 ? Positive : d < 0 ? Negative : Neutral;
        return Neutral;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
