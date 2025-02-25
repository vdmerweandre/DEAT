﻿using DEAT.Data.Models.Dtos;

namespace DEAT.AdminUI.Services.Contracts
{
    public interface ITransactionService
    {
        Task<IEnumerable<JournalEntry>> GetAllTransactionsAsync();
        Task<Guid> CreateTransactionAsync(JournalEntry transction);
        Task<Guid> ApproveTransactionAsync(Guid transactionId);
        Task<Guid> CancelTransactionAsync(Guid transactionId);
        Task<Guid> RetryTransactionAsync(Guid transactionId);
        Task<Guid> ConfirmTransactionLegAsync(Guid transactionId, Guid transactionLegId);
    }
}
