using Domain.Models.ApplicationConfigurationModels;

namespace Domain.Interfaces.ApplicationConfigurationInterfaces
{
    public interface IAccountServices
    {
        Task<UserSessionModel?> GetUserSession();
        Task SetUserSession(UserSessionModel userSession);
        Task RemoveUserSession();
        Task SetUserPreferences(UserSessionModel userSession);
    }
}
