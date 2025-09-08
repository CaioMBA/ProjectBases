using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Data.Database.EntityFrameworkContexts.Converters
{
    public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan>
    {
        public TimeOnlyConverter() : base(
            timeOnly => timeOnly.ToTimeSpan(),

            timeSpan => TimeOnly.FromTimeSpan(timeSpan))
        {
        }
    }

    public class NullableTimeOnlyConverter : ValueConverter<TimeOnly?, TimeSpan?>
    {
        public NullableTimeOnlyConverter() : base(
            timeOnly => timeOnly.HasValue ? timeOnly.Value.ToTimeSpan() : null,

            timeSpan => timeSpan.HasValue ? TimeOnly.FromTimeSpan(timeSpan.Value) : null)
        {
        }
    }
}
