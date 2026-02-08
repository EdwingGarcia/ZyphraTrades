using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Application.Abstractions;

public interface ITradeRepository
{
    Task<IReadOnlyList<Trade>> GetAllAsync(CancellationToken ct = default);
    Task<Trade?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Trade trade, CancellationToken ct = default);
    Task UpdateAsync(Trade trade, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
