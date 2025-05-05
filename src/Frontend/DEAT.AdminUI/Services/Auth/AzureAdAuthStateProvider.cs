using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace DEAT.AdminUI.Services.Auth
{
    public class AzureAdAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILogger<AzureAdAuthStateProvider> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAccessTokenProvider _accessTokenProvider;

        public AzureAdAuthStateProvider(
            ILogger<AzureAdAuthStateProvider> logger,
            IConfiguration configuration,
            IAccessTokenProvider accessTokenProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _accessTokenProvider = accessTokenProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _accessTokenProvider.RequestAccessToken();
                if (token.TryGetToken(out var accessToken))
                {
                    var claims = ParseClaimsFromJwt(accessToken.Value);
                    var identity = new ClaimsIdentity(claims, "Azure AD");
                    var user = new ClaimsPrincipal(identity);
                    return new AuthenticationState(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting authentication state");
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                keyValuePairs.TryGetValue("roles", out var roles);
                if (roles != null)
                {
                    if (roles.ToString()!.StartsWith("["))
                    {
                        var parsedRoles = System.Text.Json.JsonSerializer.Deserialize<string[]>(roles.ToString()!);
                        if (parsedRoles != null)
                        {
                            claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
                    }
                }

                keyValuePairs.TryGetValue("name", out var name);
                if (name != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, name.ToString()!));
                }

                keyValuePairs.TryGetValue("email", out var email);
                if (email != null)
                {
                    claims.Add(new Claim(ClaimTypes.Email, email.ToString()!));
                }
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
} 