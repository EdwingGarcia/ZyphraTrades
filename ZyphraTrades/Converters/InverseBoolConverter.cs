using System.Globalization;
using System.Windows.Data;

namespace ZyphraTrades.Presentation.Converters;

/// <summary>
/// Inverts a boolean value. Used for RadioButton groups bound to a single bool property.
/// true → false, false → true.
/// </summary>
public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : value;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : value;
}
