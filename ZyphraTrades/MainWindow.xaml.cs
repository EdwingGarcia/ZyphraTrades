using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ZyphraTrades.Presentation;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        Loaded += async (_, _) =>
        {
            // Cargar settings primero para que NewTrade tenga la config lista
            await vm.Settings.LoadAsync();
            await vm.TradeForm.LoadDynamicSectionsAsync();
            await vm.LoadAsync();
        };
    }

    private void TradesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not MainViewModel vm || vm.SelectedTrade == null)
            return;

        // Ensure the double-click was on a data row, not header or empty space
        var dep = (DependencyObject)e.OriginalSource;
        while (dep != null && dep is not DataGridRow)
            dep = VisualTreeHelper.GetParent(dep);

        if (dep is DataGridRow)
        {
            var detail = new TradeDetailWindow(vm.SelectedTrade) { Owner = this };
            detail.ShowDialog();
        }
    }
}
