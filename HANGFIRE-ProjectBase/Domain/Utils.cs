using Domain.Models.ApplicationConfigurationModels;
using Microsoft.Extensions.Caching.Memory;

namespace Domain
{
    public class Utils
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IMemoryCache _cache;
        public Utils(AppSettingsModel appSettings, IMemoryCache cache)
        {
            _appSettings = appSettings;
            _cache = cache;
        }

        public DataBaseConnectionModel GetDataBase(string DataBaseID)
        {
            DataBaseConnectionModel? Conection = (from v in _appSettings.DataBaseConnections
                                                  where v.DataBaseID.Equals(DataBaseID, StringComparison.OrdinalIgnoreCase)
                                                  select v).FirstOrDefault();
            if (Conection == null)
            {
                throw new InvalidOperationException($"Database not found using Id: {DataBaseID}");
            }

            return Conection;
        }

        public ApiConnectionModel GetApi(string ApiID)
        {
            ApiConnectionModel? Conection = (from v in _appSettings.ApiConnections
                                             where v.ApiID.Equals(ApiID, StringComparison.OrdinalIgnoreCase)
                                             select v).FirstOrDefault();
            if (Conection == null)
            {
                throw new InvalidOperationException($"API not found using Id: {ApiID}");
            }

            return Conection;
        }

        public ApiEndPointConnectionModel GetApiEndPoint(string ApiID, string ApiEndPointID)
        {
            ApiConnectionModel? Conection = GetApi(ApiID);
            ApiEndPointConnectionModel? EndPoint = (from v in Conection.EndPoints
                                                    where v.EndPointID.Equals(ApiEndPointID, StringComparison.OrdinalIgnoreCase)
                                                    select v).FirstOrDefault();
            if (EndPoint == null)
            {
                throw new InvalidOperationException($"API EndPoint not found using Id: {ApiEndPointID}");
            }
            return EndPoint;
        }

        public T? GetMemoryObject<T>(string key)
        {
            return _cache.TryGetValue(key, out T? cachedValue) ? cachedValue : default;
        }

        public void SetMemoryObject<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime ?? TimeSpan.FromMinutes(60)
            };

            _cache.Set(key, value, cacheEntryOptions);
        }

        public void RemoveMemoryObject(string key)
        {
            _cache.Remove(key);
        }

        public string? GetMachineName()
        {
            return System.Environment.MachineName;
        }

        public string? GetMachineHostName()
        {
            return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).HostName;
        }
    }
}
