using Domain.Enums;
using Domain.Extensions;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;
using System.Security.Cryptography;

namespace Domain
{
    public class AppUtils
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IPlatformSpecificServices _platformService;
        public AppUtils(AppSettingsModel appSettings, IPlatformSpecificServices platformService)
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

        public string GetSystemTheme()
        {
            return AppInfo.Current.RequestedTheme switch
            {
                AppTheme.Light => "light",
                AppTheme.Dark => "dark",
                AppTheme.Unspecified => "light",
                _ => "light"
            };
        }

        public async Task<string?> GetFromSecurityStorage(SecurityStorageVariablesEnum Enum)
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

        public async Task SetToSecurityStorage(SecurityStorageVariablesEnum Enum, string Value)
        {
            string cryptedValue = Value.Encrypt();
            await SecureStorage.Default.SetAsync(Enum.ToString(), cryptedValue);
        }

        public void RemoveFromSecurityStorage(SecurityStorageVariablesEnum Enum)
        {
            SecureStorage.Default.Remove(Enum.ToString());
        }

        public void ClearSecurityStorage()
        {
            SecureStorage.Default.RemoveAll();
        }

        public void SetToPreferences(PreferenceVariablesEnum Enum, string Value)
        {
            string cryptedValue = Value.Encrypt();
            Preferences.Set(Enum.ToString(), cryptedValue);
        }

        public string? GetFromPreferences(PreferenceVariablesEnum Enum)
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

        public void RemoveFromPreferences(PreferenceVariablesEnum Enum)
        {
            Preferences.Remove(Enum.ToString());
        }

        public void ClearPreferences()
        {
            Preferences.Clear();
        }

        public void OpenUrl(string Url)
        {
            Launcher.OpenAsync(new Uri(Url));
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
    }
}
