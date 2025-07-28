using Dapper;
using System.Data;

namespace Data.Database.DapperHandlers
{
    public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly?>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly? value)
        {
            parameter.DbType = DbType.Time;
            parameter.Value = value.HasValue ? new TimeSpan(value.Value.Hour, value.Value.Minute, value.Value.Second) : DBNull.Value;
        }

        public override TimeOnly? Parse(object value)
        {
            return value switch
            {
                TimeSpan timeSpan => TimeOnly.FromTimeSpan(timeSpan),
                string str when TimeSpan.TryParse(str, out TimeSpan ts) => TimeOnly.FromTimeSpan(ts),
                _ => throw new DataException($"Cannot convert {value?.GetType()} to TimeOnly.")
            };
        }
    }
}
