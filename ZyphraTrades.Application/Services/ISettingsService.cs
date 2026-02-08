using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Application.Services;

/// <summary>
/// Manages user settings (timeframes, preferences) and checklist rules.
/// </summary>
public interface ISettingsService
{
    // ── User Settings ──
    Task<UserSettings> GetSettingsAsync(CancellationToken ct = default);
    Task<UserSettings> SaveSettingsAsync(UserSettings settings, CancellationToken ct = default);
    Task<List<string>> GetTimeframesAsync(CancellationToken ct = default);
    Task SetTimeframesAsync(IEnumerable<string> timeframes, CancellationToken ct = default);

    // ── Checklist Rules ──
    Task<IReadOnlyList<ChecklistRule>> GetAllRulesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ChecklistRule>> GetActiveRulesAsync(CancellationToken ct = default);
    Task<ChecklistRule> CreateRuleAsync(string name, string? description, string? category, CancellationToken ct = default);
    Task<ChecklistRule> UpdateRuleAsync(Guid id, string name, string? description, string? category, bool isActive, int sortOrder, CancellationToken ct = default);
    Task DeleteRuleAsync(Guid id, CancellationToken ct = default);
}
