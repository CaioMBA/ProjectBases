using Domain;
using Domain.Enums;
using Domain.Extensions;
using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;

namespace Services.AuthenticationServices
{
    public class AccountServices : IAccountServices
    {
        private readonly AppUtils _utils;
        private readonly ISettingsServices _settings;
        public AccountServices(AppUtils utils, ISettingsServices settings)
        {
            _utils = utils;
            _settings = settings;
        }

        public async Task<UserSessionModel?> GetUserSession()
        {
            string? userSessionJson = await _utils.GetFromSecurityStorage(SecurityStorageVariables.UserSession);
            if (!string.IsNullOrEmpty(userSessionJson))
            {
                var user = userSessionJson.ToObject<UserSessionModel>()!;
                if (user == null)
                {
                    return null;
                }
                return user;
            }
            return null;
        }

        public async Task SetUserSession(UserSessionModel userSession)
        {
            await _utils.SetToSecurityStorage(SecurityStorageVariables.UserSession, userSession.ToJson());
        }

        public void RemoveUserSession()
        {
            _utils.RemoveFromSecurityStorage(SecurityStorageVariables.UserSession);
        }

        public void SetUserPreferences(UserSessionModel userSession)
        {
            if (!String.IsNullOrEmpty(userSession.Language))
            {
                _utils.SetToPreferences(PreferenceVariables.Language, userSession.Language);
                _settings.ChangeLanguage(userSession.Language);
            }
            if (!String.IsNullOrEmpty(userSession.Theme))
            {
                _utils.SetToPreferences(PreferenceVariables.Theme, userSession.Theme);
                _settings.ChangeTheme(userSession.Theme);
            }
        }
    }
}
