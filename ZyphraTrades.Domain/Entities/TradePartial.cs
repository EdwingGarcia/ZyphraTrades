namespace ZyphraTrades.Domain.Entities;

/// <summary>
/// Represents a partial take-profit closure within a trade.
/// Multiple partials can exist per trade to track scaling out.
/// </summary>
public class TradePartial
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Parent trade.</summary>
    public Guid TradeId { get; set; }
    public Trade? Trade { get; set; }

    /// <summary>Exit price for this partial closure.</summary>
    public decimal ExitPrice { get; set; }

    /// <summary>Size/quantity closed in this partial (lots or units).</summary>
    public decimal Quantity { get; set; }

    /// <summary>Realized PnL for this partial.</summary>
    public decimal RealizedPnl { get; set; }

    /// <summary>Percentage of original position closed (0-100).</summary>
    public decimal PercentClosed { get; set; }

    /// <summary>When this partial was executed.</summary>
    public DateTimeOffset ClosedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Whether stop-loss was moved to breakeven after this partial.</summary>
    public bool MovedToBreakeven { get; set; }

    /// <summary>Optional notes for this partial.</summary>
    public string? Notes { get; set; }
}
