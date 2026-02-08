using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Application.Abstractions;

public interface IUserSettingsRepository
{
    Task<UserSettings?> GetAsync(CancellationToken ct = default);
    Task UpsertAsync(UserSettings settings, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
