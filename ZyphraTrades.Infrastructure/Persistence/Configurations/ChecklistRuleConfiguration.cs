using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Infrastructure.Persistence.Converters;

namespace ZyphraTrades.Infrastructure.Persistence.Configurations;

public class ChecklistRuleConfiguration : IEntityTypeConfiguration<ChecklistRule>
{
    public void Configure(EntityTypeBuilder<ChecklistRule> b)
    {
        b.ToTable("ChecklistRules");
        b.HasKey(x => x.Id);

        var dtConverter = new DateTimeOffsetConverter();

        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1000);
        b.Property(x => x.Category).HasMaxLength(100);
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.SortOrder).IsRequired();
        b.Property(x => x.CreatedAt).HasConversion(dtConverter);
    }
}
