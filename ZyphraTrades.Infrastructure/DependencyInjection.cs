using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZyphraTrades.Application.Abstractions;
using ZyphraTrades.Application.Services;
using ZyphraTrades.Infrastructure.Persistence;
using ZyphraTrades.Infrastructure.Repositories;

namespace ZyphraTrades.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string sqliteConnectionString)
    {
        services.AddDbContext<AppDbContext>(
            opt => opt.UseSqlite(sqliteConnectionString),
            ServiceLifetime.Singleton);

        // Repositories
        services.AddSingleton<ITradeRepository, TradeRepository>();
        services.AddSingleton<IUserSettingsRepository, UserSettingsRepository>();
        services.AddSingleton<IChecklistRuleRepository, ChecklistRuleRepository>();

        // Application Services
        services.AddSingleton<ITradeService, TradeService>();
        services.AddSingleton<ISettingsService, SettingsService>();

        return services;
    }
}
