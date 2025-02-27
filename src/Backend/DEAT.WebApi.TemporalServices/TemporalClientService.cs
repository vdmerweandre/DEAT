using DEAT.Data.Models.Dtos;
using DEAT.WebApi.TemporalServices;
using System.Linq.Expressions;
using Temporalio.Client;

public class TemporalClientService
{
    private readonly TemporalClient _client;

    public TemporalClientService(TemporalClient client)
    {
        // Connect to the Temporal server
        //_client = TemporalClient.ConnectAsync(new("localhost:7233") { Namespace = "default" }).GetAwaiter().GetResult();
        _client = client;
    }

    public async Task StartWorkflowAsync(Guid transactionId, JournalEntry transaction)
    {
        try
        {
            // Start the workflow
            var handle = await _client.StartWorkflowAsync(
            (WithdrawalWorkflow wf) => wf.RunAsync(transactionId, transaction),
            new(id: $"{transactionId}", taskQueue: "JOURNAL_TASK_QUEUE"));

            Console.WriteLine($"Started Workflow {transactionId}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Workflow execution failed: {ex.Message}");
        }
    }

    public async Task SendWfSignalAsync(Guid TransactionId, Expression<Func<WithdrawalWorkflow, Task>> signalCall)
    {
        // Get a handle to the workflow
        var workflowId = $"{TransactionId}";
        var workflowHandle = _client.GetWorkflowHandle(workflowId);

        // Send a failed signal to the workflow
        await workflowHandle.SignalAsync(signalCall);
    }
}
