namespace ZyphraTrades.Domain.Entities;

/// <summary>
/// User-defined trading plan rule that must be verified before entering a trade.
/// e.g. "RSI en sobreventa", "ChoCH en 1m", "Estructura en 4H alineada".
/// </summary>
public class ChecklistRule
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Short name of the rule.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Detailed description / criteria.</summary>
    public string? Description { get; set; }

    /// <summary>Category grouping: "Entry", "Confirmation", "Exit", "Risk", etc.</summary>
    public string? Category { get; set; }

    /// <summary>Whether this rule is currently active in the checklist.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Display order in the checklist.</summary>
    public int SortOrder { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
