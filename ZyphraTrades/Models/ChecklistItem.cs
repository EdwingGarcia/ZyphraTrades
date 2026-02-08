using ZyphraTrades.Presentation.ViewModels;

namespace ZyphraTrades.Presentation.Models;

/// <summary>
/// View-model for a single checklist rule item in the trade form.
/// Rendered dynamically from the user's active checklist rules.
/// </summary>
public sealed class ChecklistItem : ViewModelBase
{
    public Guid RuleId { get; }
    public string RuleName { get; }
    public string? Category { get; }
    public string? Description { get; }

    private bool _isChecked;
    public bool IsChecked { get => _isChecked; set => SetProperty(ref _isChecked, value); }

    private string? _notes;
    public string? Notes { get => _notes; set => SetProperty(ref _notes, value); }

    public ChecklistItem(Guid ruleId, string ruleName, string? category, string? description)
    {
        RuleId = ruleId;
        RuleName = ruleName;
        Category = category;
        Description = description;
    }
}
