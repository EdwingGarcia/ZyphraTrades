using Microsoft.EntityFrameworkCore;
using ZyphraTrades.Application.Abstractions;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Infrastructure.Persistence;

namespace ZyphraTrades.Infrastructure.Repositories;

public class TradeRepository : ITradeRepository
{
    private readonly AppDbContext _db;
    public TradeRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Trade>> GetAllAsync(CancellationToken ct = default)
        => await _db.Trades
            .AsNoTracking()
            .OrderByDescending(t => t.OpenedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Trade trade, CancellationToken ct = default)
        => await _db.Trades.AddAsync(trade, ct);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Trades.FindAsync(new object?[] { id }, ct);
        if (entity == null) return;
        _db.Trades.Remove(entity);
    }

}
