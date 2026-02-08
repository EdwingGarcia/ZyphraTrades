using System.Collections.ObjectModel;
using System.Windows.Input;
using ZyphraTrades.Application.Services;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Presentation.Commands;

namespace ZyphraTrades.Presentation.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _svc;

    // ═══════════════════════ Timeframes ═══════════════════════

    public ObservableCollection<string> Timeframes { get; } = new();

    private string _newTimeframe = "";
    public string NewTimeframe { get => _newTimeframe; set => SetProperty(ref _newTimeframe, value); }

    private string? _selectedTimeframe;
    public string? SelectedTimeframe { get => _selectedTimeframe; set => SetProperty(ref _selectedTimeframe, value); }

    // ═══════════════════════ Checklist Rules ═══════════════════════

    public ObservableCollection<ChecklistRule> ChecklistRules { get; } = new();

    private ChecklistRule? _selectedRule;
    public ChecklistRule? SelectedRule
    {
        get => _selectedRule;
        set
        {
            if (SetProperty(ref _selectedRule, value) && value is not null)
            {
                EditRuleName = value.Name;
                EditRuleDescription = value.Description ?? "";
                EditRuleCategory = value.Category ?? "";
                EditRuleIsActive = value.IsActive;
            }
        }
    }

    // ── New Rule Fields ──
    private string _newRuleName = "";
    public string NewRuleName { get => _newRuleName; set => SetProperty(ref _newRuleName, value); }

    private string _newRuleDescription = "";
    public string NewRuleDescription { get => _newRuleDescription; set => SetProperty(ref _newRuleDescription, value); }

    private string _newRuleCategory = "";
    public string NewRuleCategory { get => _newRuleCategory; set => SetProperty(ref _newRuleCategory, value); }

    // ── Edit Rule Fields ──
    private string _editRuleName = "";
    public string EditRuleName { get => _editRuleName; set => SetProperty(ref _editRuleName, value); }

    private string _editRuleDescription = "";
    public string EditRuleDescription { get => _editRuleDescription; set => SetProperty(ref _editRuleDescription, value); }

    private string _editRuleCategory = "";
    public string EditRuleCategory { get => _editRuleCategory; set => SetProperty(ref _editRuleCategory, value); }

    private bool _editRuleIsActive = true;
    public bool EditRuleIsActive { get => _editRuleIsActive; set => SetProperty(ref _editRuleIsActive, value); }

    // ═══════════════════════ User Preferences ═══════════════════════

    private decimal _defaultRiskPercent = 1.0m;
    public decimal DefaultRiskPercent { get => _defaultRiskPercent; set => SetProperty(ref _defaultRiskPercent, value); }

    private decimal? _defaultAccountBalance;
    public decimal? DefaultAccountBalance { get => _defaultAccountBalance; set => SetProperty(ref _defaultAccountBalance, value); }

    private string _baseCurrency = "USD";
    public string BaseCurrency { get => _baseCurrency; set => SetProperty(ref _baseCurrency, value); }

    // ═══════════════════════ Status ═══════════════════════

    private string _statusText = "";
    public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

    // ═══════════════════════ Category Suggestions ═══════════════════════

    public IReadOnlyList<string> CategorySuggestions { get; } =
        new[] { "Entry", "Confirmation", "Exit", "Risk Management", "Confluence", "Structure", "Momentum" };

    // ═══════════════════════ Commands ═══════════════════════

    public ICommand AddTimeframeCommand { get; }
    public ICommand RemoveTimeframeCommand { get; }
    public ICommand MoveTimeframeUpCommand { get; }
    public ICommand MoveTimeframeDownCommand { get; }

    public ICommand AddRuleCommand { get; }
    public ICommand UpdateRuleCommand { get; }
    public ICommand DeleteRuleCommand { get; }
    public ICommand ToggleRuleCommand { get; }

    public ICommand SavePreferencesCommand { get; }

    // ═══════════════════════ Constructor ═══════════════════════

    public SettingsViewModel(ISettingsService settingsService)
    {
        _svc = settingsService;

        AddTimeframeCommand = new AsyncCommand(AddTimeframeAsync);
        RemoveTimeframeCommand = new AsyncCommand(RemoveTimeframeAsync);
        MoveTimeframeUpCommand = new RelayCommand(_ => MoveTimeframe(-1), _ => SelectedTimeframe is not null);
        MoveTimeframeDownCommand = new RelayCommand(_ => MoveTimeframe(1), _ => SelectedTimeframe is not null);

        AddRuleCommand = new AsyncCommand(AddRuleAsync);
        UpdateRuleCommand = new AsyncCommand(UpdateRuleAsync);
        DeleteRuleCommand = new AsyncCommand(DeleteRuleAsync);
        ToggleRuleCommand = new AsyncCommand(ToggleRuleAsync);

        SavePreferencesCommand = new AsyncCommand(SavePreferencesAsync);
    }

    // ═══════════════════════ Load ═══════════════════════

    public async Task LoadAsync()
    {
        try
        {
            var settings = await _svc.GetSettingsAsync();

            // Load timeframes
            Timeframes.Clear();
            foreach (var tf in settings.GetTimeframes())
                Timeframes.Add(tf);

            // Load preferences
            DefaultRiskPercent = settings.DefaultRiskPercent;
            DefaultAccountBalance = settings.DefaultAccountBalance;
            BaseCurrency = settings.BaseCurrency;

            // Load checklist rules
            var rules = await _svc.GetAllRulesAsync();
            ChecklistRules.Clear();
            foreach (var r in rules)
                ChecklistRules.Add(r);

            StatusText = $"Settings loaded · {Timeframes.Count} timeframes · {ChecklistRules.Count} rules";
        }
        catch (Exception ex)
        {
            StatusText = $"Error loading settings: {ex.Message}";
        }
    }

    // ═══════════════════════ Timeframe Operations ═══════════════════════

    private async Task AddTimeframeAsync()
    {
        var tf = NewTimeframe?.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(tf) || Timeframes.Contains(tf)) return;

        Timeframes.Add(tf);
        NewTimeframe = "";
        await _svc.SetTimeframesAsync(Timeframes);
        StatusText = $"✓ Added timeframe: {tf}";
    }

    private async Task RemoveTimeframeAsync()
    {
        if (SelectedTimeframe is null) return;
        var tf = SelectedTimeframe;
        Timeframes.Remove(tf);
        SelectedTimeframe = null;
        await _svc.SetTimeframesAsync(Timeframes);
        StatusText = $"✓ Removed timeframe: {tf}";
    }

    private async void MoveTimeframe(int direction)
    {
        if (SelectedTimeframe is null) return;
        var idx = Timeframes.IndexOf(SelectedTimeframe);
        var newIdx = idx + direction;
        if (newIdx < 0 || newIdx >= Timeframes.Count) return;

        Timeframes.Move(idx, newIdx);
        await _svc.SetTimeframesAsync(Timeframes);
    }

    // ═══════════════════════ Checklist Rule Operations ═══════════════════════

    private async Task AddRuleAsync()
    {
        if (string.IsNullOrWhiteSpace(NewRuleName)) return;

        var rule = await _svc.CreateRuleAsync(
            NewRuleName.Trim(),
            string.IsNullOrWhiteSpace(NewRuleDescription) ? null : NewRuleDescription.Trim(),
            string.IsNullOrWhiteSpace(NewRuleCategory) ? null : NewRuleCategory.Trim());

        ChecklistRules.Add(rule);
        NewRuleName = "";
        NewRuleDescription = "";
        NewRuleCategory = "";
        StatusText = $"✓ Rule created: {rule.Name}";
    }

    private async Task UpdateRuleAsync()
    {
        if (SelectedRule is null || string.IsNullOrWhiteSpace(EditRuleName)) return;

        var updated = await _svc.UpdateRuleAsync(
            SelectedRule.Id,
            EditRuleName.Trim(),
            string.IsNullOrWhiteSpace(EditRuleDescription) ? null : EditRuleDescription.Trim(),
            string.IsNullOrWhiteSpace(EditRuleCategory) ? null : EditRuleCategory.Trim(),
            EditRuleIsActive,
            SelectedRule.SortOrder);

        var idx = ChecklistRules.IndexOf(SelectedRule);
        if (idx >= 0)
        {
            ChecklistRules[idx] = updated;
            SelectedRule = updated;
        }

        StatusText = $"✓ Rule updated: {updated.Name}";
    }

    private async Task DeleteRuleAsync()
    {
        if (SelectedRule is null) return;
        var name = SelectedRule.Name;
        await _svc.DeleteRuleAsync(SelectedRule.Id);
        ChecklistRules.Remove(SelectedRule);
        SelectedRule = null;
        StatusText = $"✓ Rule deleted: {name}";
    }

    private async Task ToggleRuleAsync()
    {
        if (SelectedRule is null) return;

        var updated = await _svc.UpdateRuleAsync(
            SelectedRule.Id,
            SelectedRule.Name,
            SelectedRule.Description,
            SelectedRule.Category,
            !SelectedRule.IsActive,
            SelectedRule.SortOrder);

        var idx = ChecklistRules.IndexOf(SelectedRule);
        if (idx >= 0)
        {
            ChecklistRules[idx] = updated;
            SelectedRule = updated;
            EditRuleIsActive = updated.IsActive;
        }

        StatusText = $"✓ Rule {(updated.IsActive ? "activated" : "deactivated")}: {updated.Name}";
    }

    // ═══════════════════════ Preferences ═══════════════════════

    private async Task SavePreferencesAsync()
    {
        try
        {
            var settings = await _svc.GetSettingsAsync();
            settings.DefaultRiskPercent = DefaultRiskPercent;
            settings.DefaultAccountBalance = DefaultAccountBalance;
            settings.BaseCurrency = BaseCurrency;
            await _svc.SaveSettingsAsync(settings);
            StatusText = "✓ Preferences saved";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }
}
