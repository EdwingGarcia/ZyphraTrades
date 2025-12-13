using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZyphraTrades.Application.Abstractions;
using ZyphraTrades.Infrastructure.Persistence;
using ZyphraTrades.Infrastructure.Repositories;

namespace ZyphraTrades.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string sqliteConnectionString)
    {
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(sqliteConnectionString));
        services.AddScoped<ITradeRepository, TradeRepository>();
        return services;
    }
}
