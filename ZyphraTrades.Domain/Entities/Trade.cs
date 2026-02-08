using ZyphraTrades.Domain.Trading;

namespace ZyphraTrades.Domain.Entities;

public class Trade
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // ── Timing ──
    public DateTimeOffset OpenedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ClosedAt { get; set; }

    // ── Instrument ──
    public string Symbol { get; set; } = "AUDUSD";
    public TradeSide Side { get; set; }
    public string? Timeframe { get; set; }

    // ── Status ──
    public TradeStatus Status { get; set; } = TradeStatus.Open;
    public TradeOutcome? Outcome { get; set; }

    // ── Prices ──
    public decimal EntryPrice { get; set; }
    public decimal? ExitPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }

    // ── Breakeven Tracking ──
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
    public decimal? RiskRewardRatio { get; set; }

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

    // ── Navigation Properties ──
    public ICollection<TradePartial> Partials { get; set; } = new List<TradePartial>();
    public ICollection<TradeTimeframeAnalysis> TimeframeAnalyses { get; set; } = new List<TradeTimeframeAnalysis>();
    public ICollection<TradeChecklistEntry> ChecklistEntries { get; set; } = new List<TradeChecklistEntry>();

    // ── Computed ──
    public decimal TotalCosts => (Fees ?? 0) + (Commission ?? 0) + (Swap ?? 0) + (Slippage ?? 0);
    public TimeSpan? HoldDuration => ClosedAt.HasValue ? ClosedAt.Value - OpenedAt : null;
    public decimal TotalPartialPnl => Partials.Sum(p => p.RealizedPnl);
    public decimal ChecklistCompliancePercent =>
        ChecklistTotal > 0 ? Math.Round((decimal)(ChecklistScore ?? 0) / ChecklistTotal.Value * 100, 1) : 0;

    // ── Domain Methods ──
    public void Close(decimal exitPrice, decimal grossPnl, decimal? fees = null)
    {
        ExitPrice = exitPrice;
        ClosedAt = DateTimeOffset.UtcNow;
        GrossPnl = grossPnl;
        if (fees.HasValue) Fees = fees;
        NetPnl = grossPnl - TotalCosts;
        Status = TradeStatus.Closed;
        Outcome = NetPnl > 0 ? TradeOutcome.Win
                : NetPnl < 0 ? TradeOutcome.Loss
                : TradeOutcome.BreakEven;

        if (RiskAmount is > 0)
            ResultR = Math.Round(NetPnl / RiskAmount.Value, 4);
    }

    public void MoveToBreakeven()
    {
        BreakevenPrice = EntryPrice;
        BreakevenAt = DateTimeOffset.UtcNow;
        IsBreakevenActive = true;
    }

    public void AddPartial(decimal exitPrice, decimal quantity, decimal pnl, decimal percentClosed, bool moveToBreakeven = false)
    {
        var partial = new TradePartial
        {
            TradeId = Id,
            ExitPrice = exitPrice,
            Quantity = quantity,
            RealizedPnl = pnl,
            PercentClosed = percentClosed,
            MovedToBreakeven = moveToBreakeven
        };
        Partials.Add(partial);

        if (moveToBreakeven)
            MoveToBreakeven();
    }

    public void CalculateRiskReward()
    {
        if (!StopLoss.HasValue || !TakeProfit.HasValue) return;

        var risk = Math.Abs(EntryPrice - StopLoss.Value);
        var reward = Math.Abs(TakeProfit.Value - EntryPrice);

        if (risk > 0)
            RiskRewardRatio = Math.Round(reward / risk, 2);
    }

    public void CalculateChecklistScore()
    {
        if (ChecklistEntries.Count == 0) return;
        ChecklistTotal = ChecklistEntries.Count;
        ChecklistScore = ChecklistEntries.Count(e => e.IsChecked);
    }
}