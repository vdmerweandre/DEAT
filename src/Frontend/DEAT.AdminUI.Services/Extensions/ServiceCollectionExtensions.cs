using DEAT.AdminUI.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DEAT.AdminUI.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAdminUIServices(this IServiceCollection services, IConfiguration configuration)
        {
            var apiUrl = configuration["Api:Url"]!;
            services.AddHttpClient("WebApi", client =>
            {
                client.BaseAddress = new Uri(apiUrl, UriKind.Absolute);
            });
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<ILedgerService, LedgerService>();

            return services;
        }
    }
}
