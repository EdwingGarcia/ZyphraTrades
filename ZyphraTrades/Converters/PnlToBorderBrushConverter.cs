using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ZyphraTrades.Presentation.Converters;

/// <summary>
/// Convierte un valor PnL a un borde con color del tema:
/// Positivo → ThemeWinBorderB, Negativo → ThemeLossBorderB, Neutro → ThemeStrokeB.
/// </summary>
public sealed class PnlToBorderBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal d)
        {
            if (d > 0)
                return System.Windows.Application.Current.TryFindResource("ThemeWinBorderB") as SolidColorBrush
                    ?? new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81));
            if (d < 0)
                return System.Windows.Application.Current.TryFindResource("ThemeLossBorderB") as SolidColorBrush
                    ?? new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
        }
        return System.Windows.Application.Current.TryFindResource("ThemeStrokeB") as SolidColorBrush
            ?? new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
