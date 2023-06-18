using IndrivoHomework.Data;
using IndrivoHomework.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace IndrivoHomework.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly DbHelper _dbHelper;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthStateProvider(ProtectedSessionStorage protectedSessionStorage, DbHelper dbHelper)
        {
            _sessionStorage = protectedSessionStorage;
            _dbHelper = dbHelper;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSession = await _sessionStorage.GetAsync<UserSession>("UserSession");

                var user = userSession.Success ? userSession.Value : null;

                if (user == null) return await Task.FromResult(new AuthenticationState(_anonymous));

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Username)
                };

                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch (Exception)
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task UpdateAuthenticationState(UserSession? user)
        {
            ClaimsPrincipal claimsPrincipal;

            if (user != null)
            {
                await _sessionStorage.SetAsync("UserSession", user);

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Username)
                };

                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");

                claimsPrincipal = _anonymous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
