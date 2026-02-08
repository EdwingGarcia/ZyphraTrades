using Microsoft.EntityFrameworkCore;
using ZyphraTrades.Application.Abstractions;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Infrastructure.Persistence;

namespace ZyphraTrades.Infrastructure.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly AppDbContext _db;
    public UserSettingsRepository(AppDbContext db) => _db = db;

    public async Task<UserSettings?> GetAsync(CancellationToken ct = default)
        => await _db.UserSettings.FirstOrDefaultAsync(ct);

    public async Task UpsertAsync(UserSettings settings, CancellationToken ct = default)
    {
        var existing = await _db.UserSettings.FirstOrDefaultAsync(ct);
        if (existing is null)
        {
            await _db.UserSettings.AddAsync(settings, ct);
        }
        else
        {
            existing.TimeframesJson = settings.TimeframesJson;
            existing.DefaultRiskPercent = settings.DefaultRiskPercent;
            existing.DefaultAccountBalance = settings.DefaultAccountBalance;
            existing.BaseCurrency = settings.BaseCurrency;
            existing.UpdatedAt = settings.UpdatedAt;
            _db.UserSettings.Update(existing);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
