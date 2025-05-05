using DEAT.AdminUI.Components;
using DEAT.AdminUI.Services.Extensions;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using DEAT.AdminUI;
using DEAT.AdminUI.Services.Auth;
using Microsoft.AspNetCore.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure Azure AD Authentication
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://your-api-client-id/access_as_user");
});

// Configure Authorization
builder.Services.AddAuthorizationCore(options =>
{
    // Add permission-based policies
    foreach (var role in RolePermissions.DefaultPermissions.Keys)
    {
        foreach (var resource in RolePermissions.DefaultPermissions[role].Keys)
        {
            foreach (Permissions permission in Enum.GetValues(typeof(Permissions)))
            {
                if (permission != Permissions.None && permission != Permissions.All)
                {
                    options.AddPolicy($"Permission.{resource}.{permission}", policy =>
                        policy.RequireRole(role));
                }
            }
        }
    }
});

// Register custom services
builder.Services.AddScoped<AuthenticationStateProvider, AzureAdAuthStateProvider>();
builder.Services.AddScoped<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

// Configure HTTP client with authentication
builder.Services.AddHttpClient("WebApi", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetService<AuthorizationMessageHandler>()
        .ConfigureHandler(
            authorizedUrls: new[] { builder.HostEnvironment.BaseAddress },
            scopes: new[] { "api://your-api-client-id/access_as_user" }));

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("WebApi"));

// Register other services
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<NotificationService>();

await builder.Build().RunAsync();
