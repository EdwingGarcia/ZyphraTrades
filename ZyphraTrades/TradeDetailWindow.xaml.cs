using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Presentation;

public partial class TradeDetailWindow : Window
{
    public TradeDetailWindow(Trade trade)
    {
        InitializeComponent();
        DataContext = trade;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) return;
        DragMove();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OpenScreenshot_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is Trade t && !string.IsNullOrWhiteSpace(t.ScreenshotPath))
        {
            try
            {
                Process.Start(new ProcessStartInfo(t.ScreenshotPath) { UseShellExecute = true });
            }
            catch { /* silently ignore */ }
        }
    }
}
