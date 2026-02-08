using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Domain.Trading;

namespace ZyphraTrades.Application.DTOs;

public sealed class CreateTradeRequest
{
    // ── Basic ──
    public string Symbol { get; set; } = "AUDUSD";
    public TradeSide Side { get; set; } = TradeSide.Buy;
    public string? Timeframe { get; set; }
    public TradeStatus Status { get; set; } = TradeStatus.Open;

    // ── Timing ──
    public DateTimeOffset OpenedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ClosedAt { get; set; }

    // ── Prices ──
    public decimal EntryPrice { get; set; }
    public decimal? ExitPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }

    // ── Breakeven ──
    public decimal? BreakevenPrice { get; set; }
    public DateTimeOffset? BreakevenAt { get; set; }
    public bool IsBreakevenActive { get; set; }

    // ── Sizing & Costs ──
    public decimal? PositionSize { get; set; }
    public decimal? Fees { get; set; }
    public decimal? Commission { get; set; }
    public decimal? Swap { get; set; }
    public decimal? Slippage { get; set; }

    // ── PnL ──
    public decimal GrossPnl { get; set; }
    public decimal NetPnl { get; set; }

    // ── Risk Management ──
    public decimal? RiskAmount { get; set; }
    public decimal? RiskR { get; set; }
    public decimal? ResultR { get; set; }

    // ── Account ──
    public decimal? AccountBalanceBefore { get; set; }
    public decimal? AccountBalanceAfter { get; set; }

    // ── Psychology (PNL-focused) ──
    public EmotionalState? EmotionBefore { get; set; }
    public EmotionalState? EmotionAfter { get; set; }
    public int? ConfidenceLevel { get; set; }
    public string? PostTradeObservation { get; set; }
    public string? ImprovementCommitment { get; set; }

    // ── Context ──
    public string? Setup { get; set; }
    public string? Strategy { get; set; }
    public MarketCondition? MarketCondition { get; set; }
    public SessionType? Session { get; set; }
    public ConfluenceLevel? Confluence { get; set; }

    // ── Journal ──
    public string? Notes { get; set; }
    public string? MistakeNotes { get; set; }
    public string? LessonLearned { get; set; }
    public int? Rating { get; set; }
    public string? ScreenshotPath { get; set; }
    public string? Tags { get; set; }

    // ── Checklist Compliance ──
    public int? ChecklistScore { get; set; }
    public int? ChecklistTotal { get; set; }

    // ── Child Collections ──
    public List<TradePartialDto> Partials { get; set; } = new();
    public List<TradeTimeframeAnalysisDto> TimeframeAnalyses { get; set; } = new();
    public List<TradeChecklistEntryDto> ChecklistEntries { get; set; } = new();
}

// ── Nested DTOs for child collections ──

public sealed class TradePartialDto
{
    public Guid? Id { get; set; }
    public decimal ExitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal RealizedPnl { get; set; }
    public decimal PercentClosed { get; set; }
    public DateTimeOffset ClosedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool MovedToBreakeven { get; set; }
    public string? Notes { get; set; }
}

public sealed class TradeTimeframeAnalysisDto
{
    public Guid? Id { get; set; }
    public string Timeframe { get; set; } = string.Empty;
    public string? ScreenshotPath { get; set; }
    public string? Analysis { get; set; }
    public int SortOrder { get; set; }
}

public sealed class TradeChecklistEntryDto
{
    public Guid? Id { get; set; }
    public Guid ChecklistRuleId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public bool IsChecked { get; set; }
    public string? Notes { get; set; }
}
