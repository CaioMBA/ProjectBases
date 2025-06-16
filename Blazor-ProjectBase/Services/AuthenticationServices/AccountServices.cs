using Domain;
using Domain.Enums;
using Domain.Extensions;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;

namespace Services.AuthenticationServices
{
    public class AccountServices : IAccountServices
    {
        private readonly Utils _utils;
        private readonly ISettingsServices _settings;
        public AccountServices(Utils utils, ISettingsServices settings)
        {
            _utils = utils;
            _settings = settings;
        }

        public async Task<UserSessionModel?> GetUserSession()
        {
            try
            {
                string? userSessionJson = await _utils.GetFromProtectedStorage(ProtectedStorageVariable.UserSession);
                if (!string.IsNullOrEmpty(userSessionJson))
                {
                    var user = userSessionJson.ToObject<UserSessionModel>()!;
                    await SetUserPreferences(user);
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                _utils.SetLog($"Error retrieving user session: {ex.Message}", LogType.Error);
                return null;
            }
        }

        public async Task SetUserSession(UserSessionModel userSession)
        {
            await _utils.SetOrUpdateProtectedStorage(ProtectedStorageVariable.UserSession, userSession.ToJson());
        }

        public async Task RemoveUserSession()
        {
            await _utils.RemoveFromProtectedStorage(ProtectedStorageVariable.UserSession);
        }

        public async Task SetUserPreferences(UserSessionModel userSession)
        {
            if (!String.IsNullOrEmpty(userSession.Language))
            {
                await _utils.SetOrUpdateProtectedSession(ProtectedSessionVariable.Language, userSession.Language);
                _settings.ChangeLanguage(userSession.Language);
            }
            if (!String.IsNullOrEmpty(userSession.Theme))
            {
                await _utils.SetOrUpdateProtectedSession(ProtectedSessionVariable.Theme, userSession.Theme);
                _settings.ChangeTheme(userSession.Theme);
            }
        }
    }
}
