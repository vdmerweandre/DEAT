// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Temporalio.Client;
using DEAT.WebApi.TemporalServices.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        // Register Temporal client as a singleton
        services.AddSingleton(sp =>
            TemporalClient.ConnectAsync(new("localhost:7233")).GetAwaiter().GetResult());

        services.AddTemporalServices(ctx.Configuration);
        // Register activities
        //services.AddScoped<JournalActivities>();

        // Register Temporal worker as a hosted service
        services.AddHostedService<TemporalWorkerService>();
    })
    .Build();

await host.RunAsync();

