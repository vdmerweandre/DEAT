using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DEAT.WebApi.TemporalServices.Contract;
using DEAT.WebApi.TemporalServices.Activities;
using Temporalio.Worker;
using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services;
using Temporalio.Client;
namespace DEAT.WebApi.TemporalServices.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTemporalServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register Temporal client as a singleton
            services.AddSingleton<TemporalClientService>();
            services.AddSingleton(sp =>
                TemporalClient.ConnectAsync(new("localhost:7233")).GetAwaiter().GetResult());

            // Register Temporal worker as a hosted service
            services.AddHostedService<TemporalWorkerService>();

            services.AddScoped<IJournalActivities, JournalActivities>();
            services.AddScoped<JournalActivities>();



            //services.AddSingleton<TemporalClientService>();
            //services.AddScoped<IJournalActivities, JournalActivities>();
            ////services.AddScoped<JournalActivities>();

            //services.AddSingleton<IAccountService, AccountService>();
            //services.AddSingleton<IJournalService, JournalService>();
            //services.AddSingleton<ILedgerService, LedgerService>();

            return services;
        }
    }
}
