using Domain.Models.ApplicationConfigurationModels;

namespace Domain
{
    public class AppUtils
    {
        private readonly AppSettingsModel _appSettings;
        public AppUtils(AppSettingsModel appSettings)
        {
            _appSettings = appSettings;
        }

        public AppSettingsModel GetSettings()
        {
            return _appSettings;
        }

        public DataBaseConnectionModel GetDataBase(string DataBaseID)
        {
            DataBaseConnectionModel? Conection = (from v in _appSettings.DataBaseConnectionModels
                                                  where v.DataBaseID.ToUpper() == DataBaseID.ToUpper()
                                                  select v).FirstOrDefault();
            if (Conection == null)
            {
                throw new Exception($"API not found using Id: {DataBaseID}");
            }

            return Conection;
        }

        public ApiConnectionModel GetApi(string ApiID)
        {
            ApiConnectionModel? Conection = (from v in _appSettings.ApiConnections
                                             where v.ApiID.ToUpper() == ApiID.ToUpper()
                                             select v).FirstOrDefault();
            if (Conection == null)
            {
                throw new Exception($"API not found using Id: {ApiID}");
            }

            return Conection;
        }
    }
}
