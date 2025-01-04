using Domain.Models.ApplicationModels;

namespace Domain
{
    public class ApplicationUtils
    {
        private AppSettingsModel _AppSettings;
        public ApplicationUtils(AppSettingsModel AppConfig)
        {
            _AppSettings = AppConfig;
        }

        public DataBaseConnectionModel GetDataBase(string Name)
        {
            DataBaseConnectionModel? connection = (from v in _AppSettings.DataBaseConnections
                                                   where v.Name.ToUpper() == Name.ToUpper()
                                                   select v).FirstOrDefault();

            if (connection == null)
            {
                throw new InvalidOperationException($"Database {Name} not found!");
            }

            return connection;
        }

        public ApiConnectionModel GetAPI(string Name)
        {
            ApiConnectionModel? connection = (from v in _AppSettings.ApiConnections
                                              where v.Name.ToUpper() == Name.ToUpper()
                                              select v).FirstOrDefault();

            if (connection == null)
            {
                throw new InvalidOperationException($"Api {Name} not found!");
            }

            return connection;
        }
    }
}
