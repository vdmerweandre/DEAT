
using DEAT.Data.Models.Dtos;

namespace DEAT.WebAPI.Services.Contracts
{
    public interface IJournalService
    {
        Task<List<JournalEntry>> GetJournalEntriesAsync();
        Task<Guid> CreateJournalEntryAsync(JournalEntry transaction);
        Task<Guid> UpdateJournalStateAsync(Guid transactionId, string state);
        Task<bool> ApproveTransactionAsync(Guid transactionId);
        Task<bool> CancelTransactionAsync(Guid transactionId);
        Task<bool> RetryTransactionAsync(Guid transactionId);
        Task<bool> ConfirmTransactionLegAsync(Guid transactionId, Guid transactionLegId);
        Task<bool> ConfirmTransactionAsync(Guid transactionId);
    }
}
