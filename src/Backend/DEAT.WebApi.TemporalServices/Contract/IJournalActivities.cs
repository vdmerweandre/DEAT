
using DEAT.Data.Models.Dtos;
using DEAT.WebApi.TemporalServices.Models;
using Temporalio.Activities;

namespace DEAT.WebApi.TemporalServices.Contract
{
    public interface IJournalActivities
    {
        [Activity]
        Task CreateTransactionAsync(Guid TransactionId, JournalEntry transaction);
        [Activity]
        Task ApproveTransactionAsync(Guid transactionId);
        [Activity]
        Task UpdateTransactionStatusAsync(Guid transactionId, State state);
        [Activity]
        Task ProcessTransactionAsync(Guid TransactionId);
        [Activity]
        Task DebitAccountAsync(Guid TransactionId, JournalDetail JournalDetail);
        [Activity]
        Task CreditAccountAsync(Guid TransactionId, JournalDetail JournalDetail);
        [Activity]
        Task ConfirmTransactionAsync(Guid TransactionId, Guid transactionLegId);
        [Activity]
        Task CancelTransactionAsync(Guid TransactionId);
    }
}
