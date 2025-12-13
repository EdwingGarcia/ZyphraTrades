using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ZyphraTrades.Infrastructure.Persistence.Converters;

public sealed class DateTimeOffsetConverter
    : ValueConverter<DateTimeOffset, DateTime>
{
    public DateTimeOffsetConverter()
        : base(
            v => v.UtcDateTime,
            v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc))
        )
    {
    }
}
