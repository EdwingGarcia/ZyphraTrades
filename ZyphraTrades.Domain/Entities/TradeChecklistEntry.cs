namespace ZyphraTrades.Domain.Entities;

/// <summary>
/// Records whether a specific checklist rule was checked/met for a particular trade.
/// </summary>
public class TradeChecklistEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Parent trade.</summary>
    public Guid TradeId { get; set; }
    public Trade? Trade { get; set; }

    /// <summary>The checklist rule being evaluated.</summary>
    public Guid ChecklistRuleId { get; set; }
    public ChecklistRule? ChecklistRule { get; set; }

    /// <summary>Whether the rule was satisfied for this trade.</summary>
    public bool IsChecked { get; set; }

    /// <summary>Optional observation about this specific check.</summary>
    public string? Notes { get; set; }
}
