using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Application.Abstractions;

public interface IChecklistRuleRepository
{
    Task<IReadOnlyList<ChecklistRule>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ChecklistRule>> GetActiveAsync(CancellationToken ct = default);
    Task<ChecklistRule?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ChecklistRule rule, CancellationToken ct = default);
    Task UpdateAsync(ChecklistRule rule, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
