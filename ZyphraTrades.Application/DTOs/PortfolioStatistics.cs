namespace ZyphraTrades.Application.DTOs;

public sealed record PortfolioStatistics
{
    public static PortfolioStatistics Empty => new();

    // ── Counts ──
    public int TotalTrades { get; init; }
    public int OpenTrades { get; init; }
    public int ClosedTrades { get; init; }
    public int WinCount { get; init; }
    public int LossCount { get; init; }
    public int BreakEvenCount { get; init; }

    // ── Rates (0-100) ──
    public decimal WinRate { get; init; }
    public decimal LossRate { get; init; }

    // ── PnL ──
    public decimal GrossPnl { get; init; }
    public decimal NetPnl { get; init; }
    public decimal TotalFees { get; init; }

    // ── Averages ──
    public decimal AverageWin { get; init; }
    public decimal AverageLoss { get; init; }
    public decimal AverageNetPnl { get; init; }

    // ── Extremes ──
    public decimal LargestWin { get; init; }
    public decimal LargestLoss { get; init; }

    // ── Risk Metrics ──
    public decimal ProfitFactor { get; init; }
    public decimal Expectancy { get; init; }
    public decimal ExpectancyR { get; init; }
    public decimal AverageR { get; init; }
    public decimal BestR { get; init; }
    public decimal WorstR { get; init; }

    // ── Streaks ──
    public int MaxConsecutiveWins { get; init; }
    public int MaxConsecutiveLosses { get; init; }
    public int CurrentStreak { get; init; }

    // ── Drawdown ──
    public decimal MaxDrawdown { get; init; }
    public decimal MaxDrawdownPercent { get; init; }

    // ── Time ──
    public TimeSpan? AverageHoldTime { get; init; }
    public TimeSpan? LongestHoldTime { get; init; }
    public TimeSpan? ShortestHoldTime { get; init; }

    // ── Side Distribution ──
    public decimal LongWinRate { get; init; }
    public decimal ShortWinRate { get; init; }
    public int LongCount { get; init; }
    public int ShortCount { get; init; }
}
