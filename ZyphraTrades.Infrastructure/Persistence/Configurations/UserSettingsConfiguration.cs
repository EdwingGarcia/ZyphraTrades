using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Infrastructure.Persistence.Converters;

namespace ZyphraTrades.Infrastructure.Persistence.Configurations;

public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> b)
    {
        b.ToTable("UserSettings");
        b.HasKey(x => x.Id);

        var dtConverter = new DateTimeOffsetConverter();

        b.Property(x => x.TimeframesJson).HasMaxLength(2000).IsRequired();
        b.Property(x => x.DefaultRiskPercent).HasPrecision(8, 4);
        b.Property(x => x.DefaultAccountBalance).HasPrecision(18, 2);
        b.Property(x => x.BaseCurrency).HasMaxLength(10).IsRequired();
        b.Property(x => x.CreatedAt).HasConversion(dtConverter);
        b.Property(x => x.UpdatedAt).HasConversion(dtConverter);
    }
}
