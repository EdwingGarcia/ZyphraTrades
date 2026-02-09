using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ZyphraTrades.Presentation.Converters;

public class UrlToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string url || string.IsNullOrWhiteSpace(url))
            return DependencyProperty.UnsetValue;

        try
        {
            var imageUrl = ConvertToDirectImageUrl(url.Trim());
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(imageUrl, UriKind.Absolute);
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();
            return bi;
        }
        catch
        {
            return DependencyProperty.UnsetValue;
        }
    }

    private static string ConvertToDirectImageUrl(string url)
    {
        // TradingView snapshot URLs → direct S3 image
        var match = Regex.Match(url, @"tradingview\.com/x/([A-Za-z0-9]+)");
        if (match.Success)
        {
            var id = match.Groups[1].Value;
            return $"https://s3.tradingview.com/snapshots/{char.ToLower(id[0])}/{id}.png";
        }
        return url;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
