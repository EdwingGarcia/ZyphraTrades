using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Infrastructure.Persistence.Converters;

namespace ZyphraTrades.Infrastructure.Persistence.Configurations;

public class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> b)
    {
        b.ToTable("Trades");
        b.HasKey(x => x.Id);

        var dtConverter = new DateTimeOffsetConverter();

        // ── Timing ──
        b.Property(x => x.OpenedAt).HasConversion(dtConverter).IsRequired();
        b.Property(x => x.ClosedAt).HasConversion(dtConverter);

        // ── Instrument ──
        b.Property(x => x.Symbol).HasMaxLength(20).IsRequired();
        b.Property(x => x.Timeframe).HasMaxLength(10);

        // ── Status ──
        b.Property(x => x.Status);
        b.Property(x => x.Outcome);

        // ── Prices ──
        b.Property(x => x.EntryPrice).HasPrecision(18, 6);
        b.Property(x => x.ExitPrice).HasPrecision(18, 6);
        b.Property(x => x.StopLoss).HasPrecision(18, 6);
        b.Property(x => x.TakeProfit).HasPrecision(18, 6);

        // ── Sizing & Costs ──
        b.Property(x => x.PositionSize).HasPrecision(18, 6);
        b.Property(x => x.Fees).HasPrecision(18, 2);
        b.Property(x => x.Commission).HasPrecision(18, 2);
        b.Property(x => x.Swap).HasPrecision(18, 2);
        b.Property(x => x.Slippage).HasPrecision(18, 2);

        // ── PnL ──
        b.Property(x => x.GrossPnl).HasPrecision(18, 2);
        b.Property(x => x.NetPnl).HasPrecision(18, 2);

        // ── Risk Management ──
        b.Property(x => x.RiskAmount).HasPrecision(18, 2);
        b.Property(x => x.RiskR).HasPrecision(18, 4);
        b.Property(x => x.ResultR).HasPrecision(18, 4);
        b.Property(x => x.RiskRewardRatio).HasPrecision(18, 4);

        // ── Account ──
        b.Property(x => x.AccountBalanceBefore).HasPrecision(18, 2);
        b.Property(x => x.AccountBalanceAfter).HasPrecision(18, 2);

        // ── Psychology ──
        b.Property(x => x.EmotionBefore);
        b.Property(x => x.EmotionAfter);
        b.Property(x => x.ConfidenceLevel);

        // ── Context ──
        b.Property(x => x.Setup).HasMaxLength(100);
        b.Property(x => x.Strategy).HasMaxLength(100);
        b.Property(x => x.MarketCondition);
        b.Property(x => x.Session);
        b.Property(x => x.Confluence);

        // ── Breakeven ──
        b.Property(x => x.BreakevenPrice).HasPrecision(18, 6);
        b.Property(x => x.BreakevenAt).HasConversion(dtConverter);
        b.Property(x => x.IsBreakevenActive);

        // ── Psychology (enhanced) ──
        b.Property(x => x.PostTradeObservation).HasMaxLength(2000);
        b.Property(x => x.ImprovementCommitment).HasMaxLength(1000);

        // ── Journal ──
        b.Property(x => x.Notes).HasMaxLength(2000);
        b.Property(x => x.MistakeNotes).HasMaxLength(1000);
        b.Property(x => x.LessonLearned).HasMaxLength(1000);
        b.Property(x => x.Rating);
        b.Property(x => x.ScreenshotPath).HasMaxLength(500);
        b.Property(x => x.Tags).HasMaxLength(500);

        // ── Checklist Compliance ──
        b.Property(x => x.ChecklistScore);
        b.Property(x => x.ChecklistTotal);

        // ── Ignored computed properties ──
        b.Ignore(x => x.TotalCosts);
        b.Ignore(x => x.HoldDuration);
        b.Ignore(x => x.TotalPartialPnl);
        b.Ignore(x => x.ChecklistCompliancePercent);
    }
}
