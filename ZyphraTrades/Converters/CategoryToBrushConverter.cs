using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ZyphraTrades.Presentation.Converters;

/// <summary>
/// Convierte la categoría/estrategia de un trade a un color distintivo.
/// Scalping → Azul, Swing → Púrpura, Intradía → Amarillo, Position → Teal, Default → Indigo.
/// Busca los recursos del tema activo (ThemeCat*B).
/// </summary>
public sealed class CategoryToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var category = (value as string)?.Trim().ToLowerInvariant() ?? "";
        string resourceKey;

        if (category.Contains("scalp"))
            resourceKey = "ThemeCatScalpingB";
        else if (category.Contains("swing"))
            resourceKey = "ThemeCatSwingB";
        else if (category.Contains("intra") || category.Contains("day"))
            resourceKey = "ThemeCatIntradayB";
        else if (category.Contains("position") || category.Contains("largo"))
            resourceKey = "ThemeCatPositionB";
        else
            resourceKey = "ThemeCatDefaultB";

        return System.Windows.Application.Current.TryFindResource(resourceKey) as SolidColorBrush
            ?? new SolidColorBrush(Color.FromRgb(0x63, 0x66, 0xF1));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
