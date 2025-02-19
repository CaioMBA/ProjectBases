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
        public AccountServices(AppUtils utils)
        {
            _utils = utils;
        }

        public async Task<UserSessionModel?> GetUserSession()
        {
            string? userSessionJson = await _utils.GetFromSecurityStorage(SecurityStorageVariablesEnum.UserSession);
            if (!string.IsNullOrEmpty(userSessionJson))
            {
                return userSessionJson.ToObject<UserSessionModel>()!;
            }
            return null;
        }

        public async Task SetUserSession(UserSessionModel userSession)
        {
            await _utils.SetToSecurityStorage(SecurityStorageVariablesEnum.UserSession, userSession.ToJson());
        }

        public void RemoveUserSession()
        {
            _utils.RemoveFromSecurityStorage(SecurityStorageVariablesEnum.UserSession);
        }
    }
}
