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

        var dateTimeOffsetConverter = new DateTimeOffsetConverter();

        b.Property(x => x.OpenedAt)
            .HasConversion(dateTimeOffsetConverter)
            .IsRequired();

        b.Property(x => x.ClosedAt)
            .HasConversion(dateTimeOffsetConverter);

        b.Property(x => x.Symbol)
            .HasMaxLength(20)
            .IsRequired();

        b.Property(x => x.Timeframe)
            .HasMaxLength(10);

        b.Property(x => x.EntryPrice).HasPrecision(18, 6);
        b.Property(x => x.ExitPrice).HasPrecision(18, 6);
        b.Property(x => x.StopLoss).HasPrecision(18, 6);
        b.Property(x => x.TakeProfit).HasPrecision(18, 6);

        b.Property(x => x.PositionSize).HasPrecision(18, 6);
        b.Property(x => x.Fees).HasPrecision(18, 2);

        b.Property(x => x.GrossPnl).HasPrecision(18, 2);
        b.Property(x => x.NetPnl).HasPrecision(18, 2);

        b.Property(x => x.RiskR).HasPrecision(18, 4);
        b.Property(x => x.ResultR).HasPrecision(18, 4);

        b.Property(x => x.Setup).HasMaxLength(100);
        b.Property(x => x.Notes).HasMaxLength(1000);
    }
}
