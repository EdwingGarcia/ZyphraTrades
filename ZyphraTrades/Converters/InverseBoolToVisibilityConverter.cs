using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZyphraTrades.Presentation.Converters;

/// <summary>
/// Inverse BoolToVisibility: true → Collapsed, false → Visible.
/// Used to show content when a boolean flag is false.
/// </summary>
public sealed class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? Visibility.Collapsed : Visibility.Visible;
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility v)
            return v != Visibility.Visible;
        return true;
    }
}
