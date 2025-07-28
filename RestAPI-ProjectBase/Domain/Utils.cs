using Domain.Enums;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Domain
{
    public class Utils(IOptionsMonitor<AppSettingsModel> options,
                       IMemoryCache cache,
                       ILogger<Utils> logger)
    {
        private readonly AppSettingsModel _appSettings = options.CurrentValue;
        private readonly IMemoryCache _cache = cache;
        private readonly ILogger<Utils> _logger = logger;

        public AppSettingsModel GetSettings() => _appSettings;

        public DataBaseConnectionModel GetDataBase(string DataBaseID)
        {
            DataBaseConnectionModel? Conection = (from v in _appSettings.DataBaseConnections
                                                  where v.DataBaseID.Equals(DataBaseID, StringComparison.OrdinalIgnoreCase)
                                                  select v).FirstOrDefault();
            if (Conection == null)
            {
                throw new InvalidOperationException($"API not found using Id: {DataBaseID}");
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

        public void SetLog(string message, LogType type = LogType.Information)
        {
            switch (type)
            {
                case LogType.Warning:
                    _logger.LogWarning(message);
                    Trace.TraceWarning(message);
                    break;
                case LogType.Error:
                    _logger.LogError(message);
                    Trace.TraceError(message);
                    break;
                case LogType.Critical:
                    _logger.LogCritical(message);
                    Trace.TraceError(message);
                    break;
                default:
                    _logger.LogInformation(message);
                    Trace.TraceInformation(message);
                    break;
            }
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
