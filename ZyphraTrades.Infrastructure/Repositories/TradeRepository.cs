using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        => await _db.Trades.AsNoTracking()
            .OrderByDescending(t => t.OpenTime)
            .ToListAsync(ct);

    public async Task AddAsync(Trade trade, CancellationToken ct = default)
    {
        _db.Trades.Add(trade);
        await _db.SaveChangesAsync(ct);
    }
}
