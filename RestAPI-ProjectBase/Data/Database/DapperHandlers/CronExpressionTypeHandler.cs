using Dapper;
using NCrontab;
using System.Data;

namespace Data.Database.DapperHandlers
{
    public class CronExpressionTypeHandler : SqlMapper.TypeHandler<CrontabSchedule>
    {
        public override void SetValue(IDbDataParameter parameter, CrontabSchedule? value)
        {
            parameter.Value = value?.ToString();
        }

        public override CrontabSchedule? Parse(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is string cron)
            {
                try
                {
                    return CrontabSchedule.Parse(cron);
                }
                catch (CrontabException ex)
                {
                    throw new DataException($"Invalid cron expression: {cron}", ex);
                }
            }

            throw new DataException("Unexpected data type when parsing crontab schedule.");
        }
    }
}
