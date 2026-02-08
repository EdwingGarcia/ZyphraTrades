using Microsoft.EntityFrameworkCore;
using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public DbSet<ChecklistRule> ChecklistRules => Set<ChecklistRule>();
    public DbSet<TradePartial> TradePartials => Set<TradePartial>();
    public DbSet<TradeTimeframeAnalysis> TradeTimeframeAnalyses => Set<TradeTimeframeAnalysis>();
    public DbSet<TradeChecklistEntry> TradeChecklistEntries => Set<TradeChecklistEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
