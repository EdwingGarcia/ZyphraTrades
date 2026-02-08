using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Infrastructure.Persistence.Configurations;

public class TradeChecklistEntryConfiguration : IEntityTypeConfiguration<TradeChecklistEntry>
{
    public void Configure(EntityTypeBuilder<TradeChecklistEntry> b)
    {
        b.ToTable("TradeChecklistEntries");
        b.HasKey(x => x.Id);

        b.Property(x => x.IsChecked).IsRequired();
        b.Property(x => x.Notes).HasMaxLength(500);

        b.HasOne(x => x.Trade)
         .WithMany(t => t.ChecklistEntries)
         .HasForeignKey(x => x.TradeId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.ChecklistRule)
         .WithMany()
         .HasForeignKey(x => x.ChecklistRuleId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
