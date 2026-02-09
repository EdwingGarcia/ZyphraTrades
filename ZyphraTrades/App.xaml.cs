using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using ZyphraTrades.Application.Services;
using ZyphraTrades.Infrastructure;
using ZyphraTrades.Infrastructure.Persistence;
using ZyphraTrades.Presentation.ViewModels;

namespace ZyphraTrades.Presentation;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ZyphraTrades",
            "zyphra.db");

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        var conn = $"Data Source={dbPath}";

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // Infrastructure (DbContext + Repos + Services)
                services.AddInfrastructure(conn);

                // Presentation
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        // Auto-migrate
        using (var scope = _host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        // Cargar tema inicial desde ajustes
        var settingsSvc = _host.Services.GetRequiredService<ISettingsService>();
        var settings = await settingsSvc.GetSettingsAsync();
        ApplyTheme(settings.IsDarkMode);

        // Suscribir cambio de tema desde SettingsViewModel
        var mainVm = _host.Services.GetRequiredService<MainViewModel>();
        mainVm.Settings.ThemeChanged += isDark =>
        {
            Current.Dispatcher.Invoke(() => ApplyTheme(isDark));
        };

        _host.Services.GetRequiredService<MainWindow>().Show();
    }

    /// <summary>
    /// Cambia el ResourceDictionary del tema (oscuro/claro) en caliente.
    /// </summary>
    public void ApplyTheme(bool isDark)
    {
        var themeUri = new Uri(
            isDark ? "Themes/DarkTheme.xaml" : "Themes/LightTheme.xaml",
            UriKind.Relative);

        var merged = Resources.MergedDictionaries;

        // Reemplazar el primer diccionario (el tema)
        if (merged.Count > 0)
            merged[0] = new ResourceDictionary { Source = themeUri };
        else
            merged.Add(new ResourceDictionary { Source = themeUri });
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
            await _host.StopAsync();

        _host?.Dispose();
        base.OnExit(e);
    }
}
