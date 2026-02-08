using ZyphraTrades.Application.DTOs;
using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Application.Mapping;

public static class TradeMappingExtensions
{
    public static Trade ToEntity(this CreateTradeRequest req)
    {
        var trade = new Trade
        {
            Symbol = req.Symbol,
            Side = req.Side,
            Timeframe = req.Timeframe,
            Status = req.Status,
            OpenedAt = req.OpenedAt,
            ClosedAt = req.ClosedAt,
            EntryPrice = req.EntryPrice,
            ExitPrice = req.ExitPrice,
            StopLoss = req.StopLoss,
            TakeProfit = req.TakeProfit,
            BreakevenPrice = req.BreakevenPrice,
            BreakevenAt = req.BreakevenAt,
            IsBreakevenActive = req.IsBreakevenActive,
            PositionSize = req.PositionSize,
            Fees = req.Fees,
            Commission = req.Commission,
            Swap = req.Swap,
            Slippage = req.Slippage,
            GrossPnl = req.GrossPnl,
            NetPnl = req.NetPnl,
            RiskAmount = req.RiskAmount,
            RiskR = req.RiskR,
            ResultR = req.ResultR,
            AccountBalanceBefore = req.AccountBalanceBefore,
            AccountBalanceAfter = req.AccountBalanceAfter,
            EmotionBefore = req.EmotionBefore,
            EmotionAfter = req.EmotionAfter,
            ConfidenceLevel = req.ConfidenceLevel,
            PostTradeObservation = req.PostTradeObservation,
            ImprovementCommitment = req.ImprovementCommitment,
            Setup = req.Setup,
            Strategy = req.Strategy,
            MarketCondition = req.MarketCondition,
            Session = req.Session,
            Confluence = req.Confluence,
            Notes = req.Notes,
            MistakeNotes = req.MistakeNotes,
            LessonLearned = req.LessonLearned,
            Rating = req.Rating,
            ScreenshotPath = req.ScreenshotPath,
            Tags = req.Tags,
            ChecklistScore = req.ChecklistScore,
            ChecklistTotal = req.ChecklistTotal
        };

        // Map child collections
        foreach (var p in req.Partials)
        {
            trade.Partials.Add(new TradePartial
            {
                TradeId = trade.Id,
                ExitPrice = p.ExitPrice,
                Quantity = p.Quantity,
                RealizedPnl = p.RealizedPnl,
                PercentClosed = p.PercentClosed,
                ClosedAt = p.ClosedAt,
                MovedToBreakeven = p.MovedToBreakeven,
                Notes = p.Notes
            });
        }

        foreach (var a in req.TimeframeAnalyses)
        {
            trade.TimeframeAnalyses.Add(new TradeTimeframeAnalysis
            {
                TradeId = trade.Id,
                Timeframe = a.Timeframe,
                ScreenshotPath = a.ScreenshotPath,
                Analysis = a.Analysis,
                SortOrder = a.SortOrder
            });
        }

        foreach (var c in req.ChecklistEntries)
        {
            trade.ChecklistEntries.Add(new TradeChecklistEntry
            {
                TradeId = trade.Id,
                ChecklistRuleId = c.ChecklistRuleId,
                IsChecked = c.IsChecked,
                Notes = c.Notes
            });
        }

        return trade;
    }

    public static void ApplyTo(this CreateTradeRequest req, Trade trade)
    {
        trade.Symbol = req.Symbol;
        trade.Side = req.Side;
        trade.Timeframe = req.Timeframe;
        trade.Status = req.Status;
        trade.OpenedAt = req.OpenedAt;
        trade.ClosedAt = req.ClosedAt;
        trade.EntryPrice = req.EntryPrice;
        trade.ExitPrice = req.ExitPrice;
        trade.StopLoss = req.StopLoss;
        trade.TakeProfit = req.TakeProfit;
        trade.BreakevenPrice = req.BreakevenPrice;
        trade.BreakevenAt = req.BreakevenAt;
        trade.IsBreakevenActive = req.IsBreakevenActive;
        trade.PositionSize = req.PositionSize;
        trade.Fees = req.Fees;
        trade.Commission = req.Commission;
        trade.Swap = req.Swap;
        trade.Slippage = req.Slippage;
        trade.GrossPnl = req.GrossPnl;
        trade.NetPnl = req.NetPnl;
        trade.RiskAmount = req.RiskAmount;
        trade.RiskR = req.RiskR;
        trade.ResultR = req.ResultR;
        trade.AccountBalanceBefore = req.AccountBalanceBefore;
        trade.AccountBalanceAfter = req.AccountBalanceAfter;
        trade.EmotionBefore = req.EmotionBefore;
        trade.EmotionAfter = req.EmotionAfter;
        trade.ConfidenceLevel = req.ConfidenceLevel;
        trade.PostTradeObservation = req.PostTradeObservation;
        trade.ImprovementCommitment = req.ImprovementCommitment;
        trade.Setup = req.Setup;
        trade.Strategy = req.Strategy;
        trade.MarketCondition = req.MarketCondition;
        trade.Session = req.Session;
        trade.Confluence = req.Confluence;
        trade.Notes = req.Notes;
        trade.MistakeNotes = req.MistakeNotes;
        trade.LessonLearned = req.LessonLearned;
        trade.Rating = req.Rating;
        trade.ScreenshotPath = req.ScreenshotPath;
        trade.Tags = req.Tags;
        trade.ChecklistScore = req.ChecklistScore;
        trade.ChecklistTotal = req.ChecklistTotal;

        // Update child collections (replace strategy)
        trade.Partials.Clear();
        foreach (var p in req.Partials)
        {
            trade.Partials.Add(new TradePartial
            {
                TradeId = trade.Id,
                ExitPrice = p.ExitPrice,
                Quantity = p.Quantity,
                RealizedPnl = p.RealizedPnl,
                PercentClosed = p.PercentClosed,
                ClosedAt = p.ClosedAt,
                MovedToBreakeven = p.MovedToBreakeven,
                Notes = p.Notes
            });
        }

        trade.TimeframeAnalyses.Clear();
        foreach (var a in req.TimeframeAnalyses)
        {
            trade.TimeframeAnalyses.Add(new TradeTimeframeAnalysis
            {
                TradeId = trade.Id,
                Timeframe = a.Timeframe,
                ScreenshotPath = a.ScreenshotPath,
                Analysis = a.Analysis,
                SortOrder = a.SortOrder
            });
        }

        trade.ChecklistEntries.Clear();
        foreach (var c in req.ChecklistEntries)
        {
            trade.ChecklistEntries.Add(new TradeChecklistEntry
            {
                TradeId = trade.Id,
                ChecklistRuleId = c.ChecklistRuleId,
                IsChecked = c.IsChecked,
                Notes = c.Notes
            });
        }
    }

    public static CreateTradeRequest ToRequest(this Trade trade)
    {
        return new CreateTradeRequest
        {
            Symbol = trade.Symbol,
            Side = trade.Side,
            Timeframe = trade.Timeframe,
            Status = trade.Status,
            OpenedAt = trade.OpenedAt,
            ClosedAt = trade.ClosedAt,
            EntryPrice = trade.EntryPrice,
            ExitPrice = trade.ExitPrice,
            StopLoss = trade.StopLoss,
            TakeProfit = trade.TakeProfit,
            BreakevenPrice = trade.BreakevenPrice,
            BreakevenAt = trade.BreakevenAt,
            IsBreakevenActive = trade.IsBreakevenActive,
            PositionSize = trade.PositionSize,
            Fees = trade.Fees,
            Commission = trade.Commission,
            Swap = trade.Swap,
            Slippage = trade.Slippage,
            GrossPnl = trade.GrossPnl,
            NetPnl = trade.NetPnl,
            RiskAmount = trade.RiskAmount,
            RiskR = trade.RiskR,
            ResultR = trade.ResultR,
            AccountBalanceBefore = trade.AccountBalanceBefore,
            AccountBalanceAfter = trade.AccountBalanceAfter,
            EmotionBefore = trade.EmotionBefore,
            EmotionAfter = trade.EmotionAfter,
            ConfidenceLevel = trade.ConfidenceLevel,
            PostTradeObservation = trade.PostTradeObservation,
            ImprovementCommitment = trade.ImprovementCommitment,
            Setup = trade.Setup,
            Strategy = trade.Strategy,
            MarketCondition = trade.MarketCondition,
            Session = trade.Session,
            Confluence = trade.Confluence,
            Notes = trade.Notes,
            MistakeNotes = trade.MistakeNotes,
            LessonLearned = trade.LessonLearned,
            Rating = trade.Rating,
            ScreenshotPath = trade.ScreenshotPath,
            Tags = trade.Tags,
            ChecklistScore = trade.ChecklistScore,
            ChecklistTotal = trade.ChecklistTotal,
            Partials = trade.Partials.Select(p => new TradePartialDto
            {
                Id = p.Id,
                ExitPrice = p.ExitPrice,
                Quantity = p.Quantity,
                RealizedPnl = p.RealizedPnl,
                PercentClosed = p.PercentClosed,
                ClosedAt = p.ClosedAt,
                MovedToBreakeven = p.MovedToBreakeven,
                Notes = p.Notes
            }).ToList(),
            TimeframeAnalyses = trade.TimeframeAnalyses.Select(a => new TradeTimeframeAnalysisDto
            {
                Id = a.Id,
                Timeframe = a.Timeframe,
                ScreenshotPath = a.ScreenshotPath,
                Analysis = a.Analysis,
                SortOrder = a.SortOrder
            }).ToList(),
            ChecklistEntries = trade.ChecklistEntries.Select(c => new TradeChecklistEntryDto
            {
                Id = c.Id,
                ChecklistRuleId = c.ChecklistRuleId,
                RuleName = c.ChecklistRule?.Name ?? "",
                Category = c.ChecklistRule?.Category,
                IsChecked = c.IsChecked,
                Notes = c.Notes
            }).ToList()
        };
    }
}
