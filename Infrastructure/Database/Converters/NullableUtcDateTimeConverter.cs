using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Database.Converters;

public class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableUtcDateTimeConverter()
        : base(
            v => v == null ? null : (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()),
            v => v == null ? null : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
        )
    { }
}