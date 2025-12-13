using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using ZyphraTrades.Infrastructure;
using ZyphraTrades.Infrastructure.Persistence;

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
            "zyphra.db"
        );

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        var conn = $"Data Source={dbPath}";

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddInfrastructure(conn);

                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainViewModel>();
            })
            .Build();

        using (var scope = _host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        _host.Services.GetRequiredService<MainWindow>().Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
            await _host.StopAsync();

        _host?.Dispose();
        base.OnExit(e);
    }
}
