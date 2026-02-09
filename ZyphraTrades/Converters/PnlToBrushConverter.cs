using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ZyphraTrades.Presentation.Converters;

/// <summary>
/// Convierte un valor PnL decimal al color de texto del tema activo.
/// Positivo → ThemeWinTextB, Negativo → ThemeLossTextB, Neutro → ThemeMutedB.
/// </summary>
public class PnlToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush FallbackPositive = new(Color.FromRgb(0x34, 0xD3, 0x99));
    private static readonly SolidColorBrush FallbackNegative = new(Color.FromRgb(0xF8, 0x71, 0x71));
    private static readonly SolidColorBrush FallbackNeutral = new(Color.FromRgb(0x94, 0xA3, 0xB8));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal d)
        {
            if (d > 0)
                return System.Windows.Application.Current.TryFindResource("ThemeWinTextB") as SolidColorBrush ?? FallbackPositive;
            if (d < 0)
                return System.Windows.Application.Current.TryFindResource("ThemeLossTextB") as SolidColorBrush ?? FallbackNegative;
        }
        return System.Windows.Application.Current.TryFindResource("ThemeMutedB") as SolidColorBrush ?? FallbackNeutral;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
