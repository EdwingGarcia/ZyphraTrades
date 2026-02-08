using ZyphraTrades.Application.Abstractions;
using ZyphraTrades.Application.DTOs;
using ZyphraTrades.Application.Mapping;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Domain.Trading;

namespace ZyphraTrades.Application.Services;

public sealed class TradeService : ITradeService
{
    private readonly ITradeRepository _repo;

    public TradeService(ITradeRepository repo) => _repo = repo;

    public Task<IReadOnlyList<Trade>> GetAllAsync(CancellationToken ct)
        => _repo.GetAllAsync(ct);

    public Task<Trade?> GetByIdAsync(Guid id, CancellationToken ct)
        => _repo.GetByIdAsync(id, ct);

    public async Task<Trade> CreateAsync(CreateTradeRequest request, CancellationToken ct)
    {
        var trade = request.ToEntity();
        ApplyBusinessRules(trade);

        await _repo.AddAsync(trade, ct);
        await _repo.SaveChangesAsync(ct);
        return trade;
    }

    public async Task<Trade> UpdateAsync(Guid id, CreateTradeRequest request, CancellationToken ct)
    {
        var trade = await _repo.GetByIdAsync(id, ct)
                    ?? throw new InvalidOperationException($"Trade {id} not found");

        request.ApplyTo(trade);
        ApplyBusinessRules(trade);

        await _repo.UpdateAsync(trade, ct);
        await _repo.SaveChangesAsync(ct);
        return trade;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await _repo.DeleteAsync(id, ct);
        await _repo.SaveChangesAsync(ct);
    }

    // ────────────────────────── Business Rules ──────────────────────────

    private static void ApplyBusinessRules(Trade trade)
    {
        // Auto-calculate Risk/Reward ratio
        trade.CalculateRiskReward();

        // Auto-calculate total costs → NetPnl
        var totalCosts = trade.TotalCosts;
        if (totalCosts > 0 && trade.GrossPnl != 0 && trade.NetPnl == 0)
            trade.NetPnl = trade.GrossPnl - totalCosts;

        // Auto-determine outcome for closed trades
        if (trade.Status == TradeStatus.Closed || trade.ClosedAt.HasValue)
        {
            trade.Status = TradeStatus.Closed;
            trade.Outcome = trade.NetPnl > 0 ? TradeOutcome.Win
                          : trade.NetPnl < 0 ? TradeOutcome.Loss
                          : TradeOutcome.BreakEven;

            // Auto-calculate R result if risk amount is known
            if (trade.RiskAmount is > 0)
                trade.ResultR ??= Math.Round(trade.NetPnl / trade.RiskAmount.Value, 4);
        }

        // Account balance after
        if (trade.AccountBalanceBefore.HasValue && trade.NetPnl != 0)
            trade.AccountBalanceAfter ??= trade.AccountBalanceBefore.Value + trade.NetPnl;
    }

    // ────────────────────────── Statistics Engine ──────────────────────────

    public PortfolioStatistics CalculateStatistics(IReadOnlyList<Trade> trades)
    {
        if (trades.Count == 0) return PortfolioStatistics.Empty;

        var closed = trades.Where(t => t.Status != TradeStatus.Open && t.Status != TradeStatus.Cancelled).ToList();
        var open = trades.Where(t => t.Status == TradeStatus.Open).ToList();

        if (closed.Count == 0)
            return new PortfolioStatistics { TotalTrades = trades.Count, OpenTrades = open.Count };

        var wins = closed.Where(t => t.NetPnl > 0).ToList();
        var losses = closed.Where(t => t.NetPnl < 0).ToList();
        var breakEvens = closed.Where(t => t.NetPnl == 0).ToList();

        // ── Rates ──
        var winRate = (decimal)wins.Count / closed.Count;
        var lossRate = (decimal)losses.Count / closed.Count;

        // ── PnL ──
        var grossPnl = closed.Sum(t => t.GrossPnl);
        var netPnl = closed.Sum(t => t.NetPnl);
        var totalFees = closed.Sum(t => t.TotalCosts);

        // ── Averages ──
        var avgWin = wins.Count > 0 ? wins.Average(t => t.NetPnl) : 0;
        var avgLoss = losses.Count > 0 ? Math.Abs(losses.Average(t => t.NetPnl)) : 0;
        var avgNet = closed.Average(t => t.NetPnl);

        // ── Extremes ──
        var largestWin = wins.Count > 0 ? wins.Max(t => t.NetPnl) : 0;
        var largestLoss = losses.Count > 0 ? losses.Min(t => t.NetPnl) : 0;

        // ── Profit Factor ──
        var grossWins = wins.Sum(t => t.NetPnl);
        var grossLosses = Math.Abs(losses.Sum(t => t.NetPnl));
        var profitFactor = grossLosses > 0
            ? Math.Round(grossWins / grossLosses, 2)
            : grossWins > 0 ? 999.99m : 0;

        // ── Expectancy ──
        var expectancy = Math.Round(avgNet, 2);

        // ── R-based Metrics ──
        var rValues = closed
            .Where(t => t.ResultR.HasValue)
            .Select(t => t.ResultR!.Value)
            .ToList();

        var avgR = rValues.Count > 0 ? Math.Round(rValues.Average(), 4) : 0;
        var bestR = rValues.Count > 0 ? rValues.Max() : 0;
        var worstR = rValues.Count > 0 ? rValues.Min() : 0;
        var expectancyR = avgR;

        // ── Streaks ──
        var (maxConsWins, maxConsLosses, currentStreak) = CalculateStreaks(closed);

        // ── Drawdown ──
        var (maxDD, maxDDPct) = CalculateMaxDrawdown(closed);

        // ── Hold Times ──
        var holdTimes = closed
            .Where(t => t.HoldDuration.HasValue)
            .Select(t => t.HoldDuration!.Value)
            .ToList();

        var avgHold = holdTimes.Count > 0
            ? TimeSpan.FromTicks((long)holdTimes.Average(t => t.Ticks))
            : (TimeSpan?)null;

        var longestHold = holdTimes.Count > 0 ? holdTimes.Max() : (TimeSpan?)null;
        var shortestHold = holdTimes.Count > 0 ? holdTimes.Min() : (TimeSpan?)null;

        // ── Side Distribution ──
        var longs = closed.Where(t => t.Side == TradeSide.Buy).ToList();
        var shorts = closed.Where(t => t.Side == TradeSide.Sell).ToList();

        var longWinRate = longs.Count > 0
            ? (decimal)longs.Count(t => t.NetPnl > 0) / longs.Count : 0;
        var shortWinRate = shorts.Count > 0
            ? (decimal)shorts.Count(t => t.NetPnl > 0) / shorts.Count : 0;

        return new PortfolioStatistics
        {
            TotalTrades = trades.Count,
            OpenTrades = open.Count,
            ClosedTrades = closed.Count,
            WinCount = wins.Count,
            LossCount = losses.Count,
            BreakEvenCount = breakEvens.Count,
            WinRate = Math.Round(winRate * 100, 1),
            LossRate = Math.Round(lossRate * 100, 1),
            GrossPnl = Math.Round(grossPnl, 2),
            NetPnl = Math.Round(netPnl, 2),
            TotalFees = Math.Round(totalFees, 2),
            AverageWin = Math.Round(avgWin, 2),
            AverageLoss = Math.Round(avgLoss, 2),
            AverageNetPnl = Math.Round(avgNet, 2),
            LargestWin = Math.Round(largestWin, 2),
            LargestLoss = Math.Round(largestLoss, 2),
            ProfitFactor = profitFactor,
            Expectancy = expectancy,
            ExpectancyR = Math.Round(expectancyR, 4),
            AverageR = avgR,
            BestR = Math.Round(bestR, 4),
            WorstR = Math.Round(worstR, 4),
            MaxConsecutiveWins = maxConsWins,
            MaxConsecutiveLosses = maxConsLosses,
            CurrentStreak = currentStreak,
            MaxDrawdown = Math.Round(maxDD, 2),
            MaxDrawdownPercent = Math.Round(maxDDPct, 2),
            AverageHoldTime = avgHold,
            LongestHoldTime = longestHold,
            ShortestHoldTime = shortestHold,
            LongWinRate = Math.Round(longWinRate * 100, 1),
            ShortWinRate = Math.Round(shortWinRate * 100, 1),
            LongCount = longs.Count,
            ShortCount = shorts.Count
        };
    }

    public IReadOnlyList<EquityCurvePoint> CalculateEquityCurve(IReadOnlyList<Trade> trades)
    {
        var closed = trades
            .Where(t => t.Status != TradeStatus.Open)
            .OrderBy(t => t.ClosedAt ?? t.OpenedAt)
            .ToList();

        var points = new List<EquityCurvePoint>();
        decimal cumPnl = 0;
        decimal peak = 0;
        int num = 0;

        foreach (var trade in closed)
        {
            num++;
            cumPnl += trade.NetPnl;
            if (cumPnl > peak) peak = cumPnl;
            var dd = peak > 0 ? (peak - cumPnl) : 0;

            points.Add(new EquityCurvePoint(
                trade.ClosedAt ?? trade.OpenedAt,
                Math.Round(cumPnl, 2),
                Math.Round(dd, 2),
                num));
        }

        return points;
    }

    // ────────────────────────── Private Helpers ──────────────────────────

    private static (int maxWins, int maxLosses, int current) CalculateStreaks(
        IReadOnlyList<Trade> closedTrades)
    {
        int maxWins = 0, maxLosses = 0, currentWins = 0, currentLosses = 0;
        int current = 0;

        foreach (var t in closedTrades.OrderBy(t => t.ClosedAt ?? t.OpenedAt))
        {
            if (t.NetPnl > 0)
            {
                currentWins++;
                currentLosses = 0;
                current = currentWins;
                maxWins = Math.Max(maxWins, currentWins);
            }
            else if (t.NetPnl < 0)
            {
                currentLosses++;
                currentWins = 0;
                current = -currentLosses;
                maxLosses = Math.Max(maxLosses, currentLosses);
            }
            else
            {
                currentWins = 0;
                currentLosses = 0;
                current = 0;
            }
        }

        return (maxWins, maxLosses, current);
    }

    private static (decimal maxDD, decimal maxDDPct) CalculateMaxDrawdown(
        IReadOnlyList<Trade> closedTrades)
    {
        decimal cumPnl = 0, peak = 0, maxDD = 0, maxDDPct = 0;

        foreach (var t in closedTrades.OrderBy(t => t.ClosedAt ?? t.OpenedAt))
        {
            cumPnl += t.NetPnl;
            if (cumPnl > peak) peak = cumPnl;

            var dd = peak - cumPnl;
            if (dd > maxDD)
            {
                maxDD = dd;
                maxDDPct = peak > 0 ? Math.Round(dd / peak * 100, 2) : 0;
            }
        }

        return (maxDD, maxDDPct);
    }
}
