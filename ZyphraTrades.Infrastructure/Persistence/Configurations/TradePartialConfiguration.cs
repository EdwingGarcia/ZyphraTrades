using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Infrastructure.Persistence.Converters;

namespace ZyphraTrades.Infrastructure.Persistence.Configurations;

public class TradePartialConfiguration : IEntityTypeConfiguration<TradePartial>
{
    public void Configure(EntityTypeBuilder<TradePartial> b)
    {
        b.ToTable("TradePartials");
        b.HasKey(x => x.Id);

        var dtConverter = new DateTimeOffsetConverter();

        b.Property(x => x.ExitPrice).HasPrecision(18, 6).IsRequired();
        b.Property(x => x.Quantity).HasPrecision(18, 6).IsRequired();
        b.Property(x => x.RealizedPnl).HasPrecision(18, 2).IsRequired();
        b.Property(x => x.PercentClosed).HasPrecision(8, 2).IsRequired();
        b.Property(x => x.ClosedAt).HasConversion(dtConverter).IsRequired();
        b.Property(x => x.MovedToBreakeven).IsRequired();
        b.Property(x => x.Notes).HasMaxLength(500);

        b.HasOne(x => x.Trade)
         .WithMany(t => t.Partials)
         .HasForeignKey(x => x.TradeId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
