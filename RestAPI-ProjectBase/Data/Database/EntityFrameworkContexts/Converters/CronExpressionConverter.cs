using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NCrontab;

namespace Data.Database.EntityFrameworkContexts.Converters
{
    public class CronExpressionConverter : ValueConverter<CrontabSchedule, string>
    {
        public CronExpressionConverter() : base(
            cron => cron.ToString(),

            str => CrontabSchedule.Parse(str))
        { }
    }

    public class NullableCronExpressionConverter : ValueConverter<CrontabSchedule?, string?>
    {
        public NullableCronExpressionConverter() : base(
            cron => cron != null ? cron.ToString() : null,
            str => !string.IsNullOrWhiteSpace(str) ? CrontabSchedule.Parse(str) : null)
        { }
    }
}
