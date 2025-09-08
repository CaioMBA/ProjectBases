using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data.Database.EntityFrameworkContexts.Converters
{
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter() : base(
            dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            dateTime => DateOnly.FromDateTime(dateTime))
        { }
    }

    public class NullableDateOnlyConverter : ValueConverter<DateOnly?, DateTime?>
    {
        public NullableDateOnlyConverter() : base(
            dateOnly => dateOnly.HasValue ? dateOnly.Value.ToDateTime(TimeOnly.MinValue) : null,
            dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null)
        { }
    }
}
