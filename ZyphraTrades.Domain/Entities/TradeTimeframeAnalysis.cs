namespace ZyphraTrades.Domain.Entities;

/// <summary>
/// Per-timeframe screenshot and analysis for a trade.
/// Dynamically generated based on the user's configured timeframes.
/// </summary>
public class TradeTimeframeAnalysis
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Parent trade.</summary>
    public Guid TradeId { get; set; }
    public Trade? Trade { get; set; }

    /// <summary>Timeframe label (e.g. "1H", "4H", "D1").</summary>
    public string Timeframe { get; set; } = string.Empty;

    /// <summary>Path to the screenshot image file.</summary>
    public string? ScreenshotPath { get; set; }

    /// <summary>Free-text analysis for this timeframe.</summary>
    public string? Analysis { get; set; }

    /// <summary>Display order matching user's TF config.</summary>
    public int SortOrder { get; set; }
}
