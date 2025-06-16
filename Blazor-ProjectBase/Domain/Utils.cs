using Domain.Enums;
using Domain.Extensions;
using Domain.Interfaces.BlazorUiInterfaces;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;


namespace Domain
{
    public class Utils
    {
        private readonly IOptionsMonitor<AppSettingsModel> _optionsMonitor;
        private readonly IHttpContextAccessor _http;
        private readonly IMemoryCache _cache;
        private readonly ILogger<Utils> _logger;
        private readonly IBlazorStorageService _blazorStorage;

        public Utils(IOptionsMonitor<AppSettingsModel> optionsMonitor,
                     IMemoryCache cache,
                     IHttpContextAccessor http,
                     ILogger<Utils> logger,
                     IBlazorStorageService blazorStorage)
        {
            _optionsMonitor = optionsMonitor;
            _http = http;
            _cache = cache;
            _logger = logger;
            _blazorStorage = blazorStorage;
        }

        public DataBaseConnectionModel GetDataBase(string DataBaseID)
        {
            DataBaseConnectionModel? Conection = (from v in _optionsMonitor.CurrentValue.DataBaseConnections
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
            ApiConnectionModel? Conection = (from v in _optionsMonitor.CurrentValue.ApiConnections
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

        public ApiEndPointConnectionModel GetApiEndpoint(ApiConnectionModel Api, string EndpointID)
        {
            ApiEndPointConnectionModel? Endpoint = (from v in Api.EndPoints
                                                    where v.EndPointID.Equals(EndpointID, StringComparison.OrdinalIgnoreCase)
                                                    select v).FirstOrDefault();
            if (Endpoint == null)
            {
                throw new InvalidOperationException($"API endpoint not found using Id: {Endpoint}");
            }

            return Endpoint;

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

        public async Task SetOrUpdateProtectedStorage(ProtectedStorageVariable key, string value)
        {
            try
            {
                await _blazorStorage.SetToProtectedStorage(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set or update protected storage for key: {Key}", key);
                throw new InvalidOperationException($"Could not set protected storage for {key}, error: {ex.Message}");
            }
        }

        public async Task<string> GetFromProtectedStorage(ProtectedStorageVariable key)
        {
            try
            {
                return await _blazorStorage.GetFromProtectedStorage(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get protected storage for key: {Key}", key);
                throw new InvalidOperationException($"Could not get protected storage for {key}, error: {ex.Message}");
            }
        }

        public async Task RemoveFromProtectedStorage(ProtectedStorageVariable key)
        {
            try
            {
                await _blazorStorage.RemoveFromProtectedStorage(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove protected storage for key: {Key}", key);
                throw new InvalidOperationException($"Could not remove protected storage for {key}, error: {ex.Message}");
            }
        }

        public async Task SetOrUpdateProtectedSession(ProtectedSessionVariable key, string value)
        {
            try
            {
                await _blazorStorage.SetToProtectedSession(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set or update protected session for key: {Key}", key);
                throw new InvalidOperationException($"Could not set protected session for {key}, error: {ex.Message}");
            }
        }

        public async Task<string> GetFromProtectedSession(ProtectedSessionVariable key)
        {
            try
            {
                return await _blazorStorage.GetFromProtectedSession(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get protected session for key: {Key}", key);
                throw new InvalidOperationException($"Could not get protected session for {key}, error: {ex.Message}");
            }
        }

        public async Task RemoveFromProtectedSession(ProtectedSessionVariable key)
        {
            try
            {
                await _blazorStorage.RemoveFromProtectedSession(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove protected session for key: {Key}", key);
                throw new InvalidOperationException($"Could not remove protected session for {key}, error: {ex.Message}");
            }
        }
    }
}
