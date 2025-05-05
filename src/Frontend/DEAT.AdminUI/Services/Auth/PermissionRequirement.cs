using Microsoft.AspNetCore.Authorization;

namespace DEAT.AdminUI.Services.Auth
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Resource { get; }
        public Permissions Permission { get; }

        public PermissionRequirement(string resource, Permissions permission)
        {
            Resource = resource;
            Permission = permission;
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userRoles = context.User.Claims
                .Where(c => c.Type == "roles")
                .Select(c => c.Value);

            foreach (var role in userRoles)
            {
                if (RolePermissions.DefaultPermissions.TryGetValue(role, out var permissions))
                {
                    if (permissions.TryGetValue(requirement.Resource, out var resourcePermissions))
                    {
                        if ((resourcePermissions & requirement.Permission) == requirement.Permission)
                        {
                            context.Succeed(requirement);
                            return Task.CompletedTask;
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
} 