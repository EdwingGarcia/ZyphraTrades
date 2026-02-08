using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ZyphraTrades.Domain.Trading;

namespace ZyphraTrades.Presentation.Converters;

public class SideToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush BuyBrush = new(Color.FromRgb(0x22, 0xC5, 0x5E));
    private static readonly SolidColorBrush SellBrush = new(Color.FromRgb(0xE1, 0x1D, 0x48));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TradeSide side)
            return side == TradeSide.Buy ? BuyBrush : SellBrush;
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
