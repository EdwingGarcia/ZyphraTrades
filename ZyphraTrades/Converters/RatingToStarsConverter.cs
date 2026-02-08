using System.Globalization;
using System.Windows.Data;

namespace ZyphraTrades.Presentation.Converters;

public class RatingToStarsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int rating && rating > 0 && rating <= 5)
            return new string('★', rating) + new string('☆', 5 - rating);
        return "☆☆☆☆☆";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
