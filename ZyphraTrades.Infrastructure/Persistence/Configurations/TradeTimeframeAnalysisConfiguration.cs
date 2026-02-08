using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Infrastructure.Persistence.Configurations;

public class TradeTimeframeAnalysisConfiguration : IEntityTypeConfiguration<TradeTimeframeAnalysis>
{
    public void Configure(EntityTypeBuilder<TradeTimeframeAnalysis> b)
    {
        b.ToTable("TradeTimeframeAnalyses");
        b.HasKey(x => x.Id);

        b.Property(x => x.Timeframe).HasMaxLength(10).IsRequired();
        b.Property(x => x.ScreenshotPath).HasMaxLength(500);
        b.Property(x => x.Analysis).HasMaxLength(2000);
        b.Property(x => x.SortOrder).IsRequired();

        b.HasOne(x => x.Trade)
         .WithMany(t => t.TimeframeAnalyses)
         .HasForeignKey(x => x.TradeId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
