using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DEAT.AdminUI.Services.Auth
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
        private IConfiguration Configuration { get; }

        public PermissionPolicyProvider(
            IOptions<AuthorizationOptions> options,
            IConfiguration configuration)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
            Configuration = configuration;
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("Permission", StringComparison.OrdinalIgnoreCase))
            {
                var parts = policyName.Split('.');
                if (parts.Length == 3)
                {
                    var resource = parts[1];
                    if (Enum.TryParse<Permissions>(parts[2], out var permission))
                    {
                        var policy = new AuthorizationPolicyBuilder();
                        policy.AddRequirements(new PermissionRequirement(resource, permission));
                        return Task.FromResult<AuthorizationPolicy?>(policy.Build());
                    }
                }
            }

            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();
    }
} 