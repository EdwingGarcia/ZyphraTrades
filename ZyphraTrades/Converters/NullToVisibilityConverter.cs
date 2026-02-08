using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZyphraTrades.Presentation.Converters;

/// <summary>
/// Returns Visible when value is NOT null, Collapsed otherwise.
/// </summary>
public sealed class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not null ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
