﻿using Domain.Interfaces.ApplicationConfigurationInterfaces;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Services.AuthenticationServices
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAccountServices _accountServices;
        private readonly ClaimsPrincipal _anonymous;
        private UserSessionModel? _currentUserSession;

        public CustomAuthenticationStateProvider(IAccountServices accountServices)
        {
            _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            _accountServices = accountServices;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authenticationState = new AuthenticationState(_anonymous);

            var UserSession = await CurrentUserSession();
            if (UserSession is not null)
            {
                _currentUserSession = UserSession;
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
                _currentUserSession = userSession;
                if (rememberUser)
                {
                    await _accountServices.SetUserSession(userSession);
                }
                claimsPrincipal = GetClaimsPrincipal(userSession);
                await _accountServices.SetUserPreferences(userSession);
            }
            else
            {
                _currentUserSession = null;
                await _accountServices.RemoveUserSession();
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task<UserSessionModel?> CurrentUserSession()
        {
            return _currentUserSession ?? await _accountServices.GetUserSession();
        }

        private static ClaimsPrincipal GetClaimsPrincipal(UserSessionModel userSession)
        {
            var claimList = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userSession?.Name ?? "anonymous"),
                new Claim(ClaimTypes.Email, userSession?.Email ?? string.Empty),
            };

            foreach (var role in userSession?.Roles ?? [])
            {
                claimList.Add(new Claim(ClaimTypes.Role, role));
            }

            var claims = new ClaimsIdentity(claimList, "MauiHybridAuth");
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }
    }
}
