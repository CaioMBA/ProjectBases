using Domain.Enums;
using Domain.Extensions;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Domain
{
    public class AppUtils
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IPlatformSpecificServices _platformService;
        public AppUtils(AppSettingsModel appSettings,
                        IPlatformSpecificServices platformService,
                        ILogger<AppUtils> logger)
        {
            _appSettings = appSettings;
            _platformService = platformService;
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

        public string GetSystemFilePath()
        {
            return FileSystem.AppDataDirectory;
        }

        public AppTheme GetSystemTheme()
        {
            return AppInfo.Current.RequestedTheme;
        }

        public async Task<string?> GetFromSecurityStorage(SecurityStorageVariables Enum)
        {
            var cryptedValue = await SecureStorage.Default.GetAsync(Enum.ToString());
            if (String.IsNullOrEmpty(cryptedValue))
            {
                return null;
            }

            try
            {
                return cryptedValue.Decrypt();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error decoding Base64: {ex.Message}");
                return null;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"Error during decryption: {ex.Message}");
                return null;
            }
        }

        public async Task SetToSecurityStorage(SecurityStorageVariables Enum, string Value)
        {
            string cryptedValue = Value.Encrypt();
            await SecureStorage.Default.SetAsync(Enum.ToString(), cryptedValue);
        }

        public void RemoveFromSecurityStorage(SecurityStorageVariables Enum)
        {
            SecureStorage.Default.Remove(Enum.ToString());
        }

        public void ClearSecurityStorage()
        {
            SecureStorage.Default.RemoveAll();
        }

        public void SetToPreferences(PreferenceVariables Enum, string Value)
        {
            string cryptedValue = Value.Encrypt();
            Preferences.Set(Enum.ToString(), cryptedValue);
        }

        public string? GetFromPreferences(PreferenceVariables Enum)
        {
            var cryptedValue = Preferences.Get(Enum.ToString(), null);
            if (String.IsNullOrEmpty(cryptedValue))
            {
                return null;
            }
            try
            {
                return cryptedValue.Decrypt();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error decoding Base64: {ex.Message}");
                return null;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"Error during decryption: {ex.Message}");
                return null;
            }
        }

        public void RemoveFromPreferences(PreferenceVariables Enum)
        {
            Preferences.Remove(Enum.ToString());
        }

        public void ClearPreferences()
        {
            Preferences.Clear();
        }

        public async Task OpenUrl(string Url)
        {
            await Launcher.OpenAsync(new Uri(Url));
        }

        public async Task<FileResult?> PickFileResult()
        {
            var options = new PickOptions
            {
                PickerTitle = "Please select a file",
            };
            return await FilePicker.PickAsync(options);
        }

        public async Task<string?> PickDirectory()
        {
            return await _platformService.PickDirectory();
        }

        public async Task OpenDirectory(string folderPath)
        {
            await _platformService.OpenDirectory(folderPath);
        }

        public async Task SendLocalNotification(string title, string message)
        {
            await _platformService.SendLocalNotification(title, message);
        }
    }
}
