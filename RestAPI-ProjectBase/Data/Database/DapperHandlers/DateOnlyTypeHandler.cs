using Dapper;
using System.Data;

namespace Data.Database.DapperHandlers
{
    public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly?>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly? value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value.HasValue ? new DateTime(value.Value.Year, value.Value.Month, value.Value.Day) : DBNull.Value;
        }

        public override DateOnly? Parse(object value)
        {
            return value switch
            {
                DateTime dateTime => DateOnly.FromDateTime(dateTime),
                string str when DateTime.TryParse(str, out var dt) => DateOnly.FromDateTime(dt),
                _ => throw new DataException($"Cannot convert {value?.GetType()} to DateOnly.")
            };
        }
    }
}
