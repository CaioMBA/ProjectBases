using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Services.AuthenticationServices
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAccountServices _accountServices;
        private readonly ClaimsPrincipal _anonymous;

        public CustomAuthenticationStateProvider(IAccountServices accountServices)
        {
            _accountServices = accountServices;
            _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authenticationState = new AuthenticationState(_anonymous);

            var UserSession = await _accountServices.GetUserSession();
            if (UserSession is not null)
            {
                var identity = GetClaimsPrincipal(UserSession);
                var user = new ClaimsPrincipal(identity);
                authenticationState = new AuthenticationState(user);
            }
            return authenticationState;
        }

        public async Task UpdateAuthenticationState(UserSessionModel? userSession, bool rememberUser = false)
        {
            ClaimsPrincipal claimsPrincipal = _anonymous;

            if (userSession is not null)
            {
                if (rememberUser)
                {
                    await _accountServices.SetUserSession(userSession);
                }
                claimsPrincipal = GetClaimsPrincipal(userSession);
            }
            else
            {
                _accountServices.RemoveUserSession();
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private ClaimsPrincipal GetClaimsPrincipal(UserSessionModel userSession)
        {
            var claims = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userSession.Name),
                    new Claim(ClaimTypes.Email, userSession.Email),
                    new Claim(ClaimTypes.Role, userSession.Role)
                }, "CustomAuth");
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }
    }
}
