using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.DataAccess.Database.Converters;

// Non-nullable DateTime
public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),            // model -> provider (DB)
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)                       // provider (DB) -> model
        )
    { }
}