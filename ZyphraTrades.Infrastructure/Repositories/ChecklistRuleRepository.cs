using Microsoft.EntityFrameworkCore;
using ZyphraTrades.Application.Abstractions;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Infrastructure.Persistence;

namespace ZyphraTrades.Infrastructure.Repositories;

public class ChecklistRuleRepository : IChecklistRuleRepository
{
    private readonly AppDbContext _db;
    public ChecklistRuleRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ChecklistRule>> GetAllAsync(CancellationToken ct = default)
        => await _db.ChecklistRules
            .OrderBy(r => r.SortOrder)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ChecklistRule>> GetActiveAsync(CancellationToken ct = default)
        => await _db.ChecklistRules
            .Where(r => r.IsActive)
            .OrderBy(r => r.SortOrder)
            .ToListAsync(ct);

    public async Task<ChecklistRule?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ChecklistRules.FindAsync(new object?[] { id }, ct);

    public async Task AddAsync(ChecklistRule rule, CancellationToken ct = default)
        => await _db.ChecklistRules.AddAsync(rule, ct);

    public Task UpdateAsync(ChecklistRule rule, CancellationToken ct = default)
    {
        _db.ChecklistRules.Update(rule);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.ChecklistRules.FindAsync(new object?[] { id }, ct);
        if (entity is not null)
            _db.ChecklistRules.Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
