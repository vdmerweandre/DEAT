using Microsoft.Extensions.Hosting;
using Temporalio.Client;
using Temporalio.Worker;
using DEAT.WebApi.TemporalServices;
using Microsoft.Extensions.DependencyInjection;
using DEAT.WebApi.TemporalServices.Contract;

public class TemporalWorkerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TemporalClient _client;

    public TemporalWorkerService(IServiceProvider serviceProvider, TemporalClient client)
    {
        _serviceProvider = serviceProvider;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var activities = scope.ServiceProvider.GetRequiredService<IJournalActivities>();

        // Create and start the Temporal worker
        using var worker = new TemporalWorker(
            _client,
            new TemporalWorkerOptions("JOURNAL_TASK_QUEUE")
                .AddAllActivities(activities)
                .AddWorkflow<WithdrawalWorkflow>());

        Console.WriteLine("Worker started...");
        await worker.ExecuteAsync(stoppingToken);
    }
}
