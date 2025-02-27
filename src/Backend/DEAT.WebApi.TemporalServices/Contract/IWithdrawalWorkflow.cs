
using DEAT.Data.Models.Dtos;
using DEAT.WebApi.TemporalServices.Models;
using Temporalio.Workflows;

namespace DEAT.WebApi.TemporalServices.Contract
{
    public interface IWithdrawalWorkflow
    {
        Task RunAsync(Guid transactionId, JournalEntry entry);

        //Commands
        Task DebitAccountAsync(Guid transactionId, JournalDetail JournalDetail);

        Task CreditAccountAsync(Guid transactionId, JournalDetail JournalDetail);

        Task ApproveTransactionAsync(Guid transactionId);

        Task ProcessTransactionAsync(Guid transactionId);

        Task ConfirmTransactionLegAsync(Guid transactionId, Guid transactionLegId);

        Task CancelTransactionAsync(Guid transactionId);


        //Events
        Task TransactionFailed(Guid transactionId);

        Task TransactionProcessed(Guid transactionId);

        Task TransactionSucceeded(Guid transactionId);

        public State GetCurrentState(); // => _currentState
    }
}
