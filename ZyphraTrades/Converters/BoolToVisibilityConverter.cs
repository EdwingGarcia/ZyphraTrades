using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZyphraTrades.Presentation.Converters;

/// <summary>
/// Convierte bool a Visibility para secciones colapsables.
/// True → Visible, False → Collapsed.
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? Visibility.Visible : Visibility.Collapsed;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility v)
            return v == Visibility.Visible;
        return false;
    }
}
