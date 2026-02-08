using ZyphraTrades.Application.Abstractions;
using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Application.Services;

public sealed class SettingsService : ISettingsService
{
    private readonly IUserSettingsRepository _settingsRepo;
    private readonly IChecklistRuleRepository _ruleRepo;

    public SettingsService(IUserSettingsRepository settingsRepo, IChecklistRuleRepository ruleRepo)
    {
        _settingsRepo = settingsRepo;
        _ruleRepo = ruleRepo;
    }

    // ═══════════════════════ User Settings ═══════════════════════

    public async Task<UserSettings> GetSettingsAsync(CancellationToken ct = default)
    {
        var settings = await _settingsRepo.GetAsync(ct);
        if (settings is not null) return settings;

        // Create default settings on first access
        settings = new UserSettings();
        await _settingsRepo.UpsertAsync(settings, ct);
        await _settingsRepo.SaveChangesAsync(ct);
        return settings;
    }

    public async Task<UserSettings> SaveSettingsAsync(UserSettings settings, CancellationToken ct = default)
    {
        settings.UpdatedAt = DateTimeOffset.UtcNow;
        await _settingsRepo.UpsertAsync(settings, ct);
        await _settingsRepo.SaveChangesAsync(ct);
        return settings;
    }

    public async Task<List<string>> GetTimeframesAsync(CancellationToken ct = default)
    {
        var settings = await GetSettingsAsync(ct);
        return settings.GetTimeframes();
    }

    public async Task SetTimeframesAsync(IEnumerable<string> timeframes, CancellationToken ct = default)
    {
        var settings = await GetSettingsAsync(ct);
        settings.SetTimeframes(timeframes);
        await SaveSettingsAsync(settings, ct);
    }

    // ═══════════════════════ Checklist Rules ═══════════════════════

    public Task<IReadOnlyList<ChecklistRule>> GetAllRulesAsync(CancellationToken ct = default)
        => _ruleRepo.GetAllAsync(ct);

    public Task<IReadOnlyList<ChecklistRule>> GetActiveRulesAsync(CancellationToken ct = default)
        => _ruleRepo.GetActiveAsync(ct);

    public async Task<ChecklistRule> CreateRuleAsync(string name, string? description, string? category, CancellationToken ct = default)
    {
        var allRules = await _ruleRepo.GetAllAsync(ct);
        var rule = new ChecklistRule
        {
            Name = name,
            Description = description,
            Category = category,
            SortOrder = allRules.Count + 1
        };

        await _ruleRepo.AddAsync(rule, ct);
        await _ruleRepo.SaveChangesAsync(ct);
        return rule;
    }

    public async Task<ChecklistRule> UpdateRuleAsync(Guid id, string name, string? description, string? category, bool isActive, int sortOrder, CancellationToken ct = default)
    {
        var rule = await _ruleRepo.GetByIdAsync(id, ct)
                   ?? throw new InvalidOperationException($"ChecklistRule {id} not found");

        rule.Name = name;
        rule.Description = description;
        rule.Category = category;
        rule.IsActive = isActive;
        rule.SortOrder = sortOrder;

        await _ruleRepo.UpdateAsync(rule, ct);
        await _ruleRepo.SaveChangesAsync(ct);
        return rule;
    }

    public async Task DeleteRuleAsync(Guid id, CancellationToken ct = default)
    {
        await _ruleRepo.DeleteAsync(id, ct);
        await _ruleRepo.SaveChangesAsync(ct);
    }
}
