using DEAT.Data.Models.Dtos;
using DEAT.WebApi.TemporalServices.Contract;
using Microsoft.Extensions.Logging;
using Temporalio.Exceptions;
using Temporalio.Workflows;
using State = DEAT.WebApi.TemporalServices.Models.State;

namespace DEAT.WebApi.TemporalServices
{
    [Workflow]
    public class WithdrawalWorkflow : IWithdrawalWorkflow
    {
        private readonly TaskCompletionSource<string> _approvalSignalReceived = new();
        private readonly TaskCompletionSource<string> _processedSignalReceived = new();
        private readonly TaskCompletionSource<string> _confirmationSignalReceived = new();
        private readonly TaskCompletionSource<string> _completionSignalReceived = new();

        private State _currentState = State.Created;

        private ActivityOptions activityOptions = new ActivityOptions { 
            StartToCloseTimeout = TimeSpan.FromSeconds(5), 
            RetryPolicy = new Temporalio.Common.RetryPolicy
            {
                InitialInterval = TimeSpan.FromSeconds(1),
                MaximumInterval = TimeSpan.FromSeconds(100),
                BackoffCoefficient = 2,
                MaximumAttempts = 3,
                NonRetryableErrorTypes = new[] { "InvalidAccountException", "InsufficientFundsException" }
            }
        };


        [WorkflowQuery]
        public State GetCurrentState() => _currentState;

        [WorkflowRun]
        public async Task RunAsync(Guid transactionId, JournalEntry entry)
        {
            Workflow.Logger.Log(LogLevel.Information, $"Workflow started.");

            try
            {
                await Workflow.ExecuteActivityAsync<IJournalActivities>(
                    activities => activities.CreateTransactionAsync(transactionId, entry),
                    activityOptions);

                while (true)
                {
                    switch (_currentState)
                    {
                        case State.Created:
                            Console.WriteLine("Waiting for approval or cancellation...");

                            Task completedTask = await Task.WhenAny(_approvalSignalReceived.Task, _completionSignalReceived.Task);
                            Console.WriteLine($"Signal received with: {completedTask}");
                            break;

                        case State.Approved:
                            Console.WriteLine("Processing tranaction...");

                            await  ProcessTransactionAsync(transactionId);
                            break;

                        case State.Processed:
                            Console.WriteLine("Waiting for processing or failure...");

                            completedTask = await Task.WhenAny(_confirmationSignalReceived.Task, _completionSignalReceived.Task);
                            Console.WriteLine($"Signal received with: {completedTask}");
                            break;

                        case State.Confirmed:
                            Console.WriteLine("Waiting for confirmation, success or failure...");

                            completedTask = await Task.WhenAny(_completionSignalReceived.Task);
                            Console.WriteLine($"Signal received with: {completedTask}");
                            break;


                        case State.Completed:
                        case State.Failed:
                        case State.Cancelled:
                            Console.WriteLine("Workflow completed.");
                            Workflow.Logger.Log(LogLevel.Information, "Workflow completed.");
                            return; // Exit the workflow loop
                    }
                }
            }
            catch (ApplicationFailureException ex) when (ex.ErrorType == "InsufficientFundsException")
            {
                throw new ApplicationFailureException("Create Withdrawal failed due to insufficient funds.", ex);
            }
        }

        [WorkflowSignal]
        public async Task DebitAccountAsync(Guid transactionId, JournalDetail JournalDetail)
        {
            await Workflow.ExecuteActivityAsync<IJournalActivities>(
                activities => activities.DebitAccountAsync(transactionId, JournalDetail),
                activityOptions);
        }

        [WorkflowSignal]
        public async Task CreditAccountAsync(Guid transactionId, JournalDetail JournalDetail)
        {
            await Workflow.ExecuteActivityAsync<IJournalActivities>(
                activities => activities.CreditAccountAsync(transactionId, JournalDetail),
                activityOptions);
        }

        [WorkflowSignal]
        public async Task TransactionProcessed(Guid transactionId)
        {
            _currentState = State.Processed;

            if (!_processedSignalReceived.Task.IsCompleted)
            {
                _processedSignalReceived.TrySetResult($"Processed signal received - {transactionId}");
            }
            await Task.FromResult( _currentState );
        }

        [WorkflowSignal]
        public async Task TransactionSucceeded(Guid transactionId)
        {
            _currentState = State.Completed;

            await Workflow.ExecuteActivityAsync<IJournalActivities>(
                activities => activities.UpdateTransactionStatusAsync(transactionId, _currentState),
                activityOptions);

            if (!_completionSignalReceived.Task.IsCompleted)
            {
                _completionSignalReceived.TrySetResult($"Completed signal received - {transactionId}");
            }
            await Task.FromResult(_currentState);
        }

        [WorkflowSignal]
        public async Task TransactionFailed(Guid transactionId)
        {
            _currentState = State.Failed;

            if (!_completionSignalReceived.Task.IsCompleted)
            {
                _completionSignalReceived.TrySetResult($"Failed signal received - {transactionId}");
            }
            await Task.FromResult(_currentState);
        }

        [WorkflowSignal]
        public async Task TransactionCancelled(Guid transactionId)
        {
            _currentState = State.Cancelled;

            if (!_completionSignalReceived.Task.IsCompleted)
            {
                _completionSignalReceived.TrySetResult($"Cancelled signal received - {transactionId}");
            }
            await Task.FromResult(_currentState);
        }
        
        [WorkflowSignal]
        public async Task TransactionApproved(Guid transactionId)
        {
            _currentState = State.Approved;

            if (!_approvalSignalReceived.Task.IsCompleted)
            {
                _approvalSignalReceived.TrySetResult($"Approved signal received - {transactionId}");
            }
            await Task.FromResult(_currentState);
        }

        [WorkflowSignal]
        public async Task ApproveTransactionAsync(Guid transactionId)
        {
            await Workflow.ExecuteActivityAsync<IJournalActivities>(
                activities => activities.ApproveTransactionAsync(transactionId),
                activityOptions);
        }

        [WorkflowSignal]
        public async Task ProcessTransactionAsync(Guid transactionId)
        {
            await Workflow.ExecuteActivityAsync<IJournalActivities>(
                activities => activities.ProcessTransactionAsync(transactionId),
                activityOptions);
        }

        [WorkflowSignal]
        public async Task ConfirmTransactionLegAsync(Guid transactionId, Guid transactionLegId)
        {
            await Workflow.ExecuteActivityAsync<IJournalActivities>(
                activities => activities.ConfirmTransactionAsync(transactionId, transactionLegId),
                activityOptions);
        }

        [WorkflowSignal]
        public async Task CancelTransactionAsync(Guid transactionId)
        {
            await Workflow.ExecuteActivityAsync<IJournalActivities>(
                activities => activities.CancelTransactionAsync(transactionId),
                activityOptions);
        }
    }
}
