using DEAT.Data.Models.Dtos;
using DEAT.WebApi.TemporalServices.Contract;
using DEAT.WebApi.TemporalServices.Models;
using DEAT.WebAPI.Services.Contracts;
using Temporalio.Activities;

namespace DEAT.WebApi.TemporalServices.Activities
{
    public class JournalActivities: IJournalActivities
    {
        private readonly IJournalService _journalService;
        private readonly IAccountService _accountService;
        private readonly ILedgerService _ledgerService;
        private TemporalClientService _temporalClientService;

        public JournalActivities(
            IJournalService transactionService,
            IAccountService accountService,
            ILedgerService ledgerService,
            TemporalClientService temporalClientService)
        {
            _journalService = transactionService;
            _ledgerService = ledgerService; 
            _accountService = accountService;   
            _temporalClientService = temporalClientService;
        }

        [Activity]
        public async Task CreateTransactionAsync(Guid transactionId, JournalEntry transaction)
        {
            await _journalService.CreateJournalEntryAsync(transactionId, transaction);
        }

        [Activity]
        public async Task ApproveTransactionAsync(Guid transactionId)
        {
            var success = await _journalService.ApproveTransactionAsync(transactionId);

            if (success)
            {
                await _temporalClientService.SendWfSignalAsync(transactionId, wf => wf.TransactionApproved(transactionId));
            }
        }

        [Activity]
        public async Task ConfirmTransactionAsync(Guid TransactionId, Guid transactionLegId)
        {
            var success = await _journalService.ConfirmTransactionLegAsync(TransactionId, transactionLegId);

            success = success && await _journalService.ConfirmTransactionAsync(TransactionId);

            if (success)
            {
                await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.TransactionSucceeded(TransactionId));
            }
        }

        [Activity]
        public async Task CancelTransactionAsync(Guid TransactionId)
        {
            var success = await _journalService.CancelTransactionAsync(TransactionId);

            if (success)
            {
                await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.TransactionCancelled(TransactionId));
            }
        }

        [Activity]
        public async Task CreditAccountAsync(Guid TransactionId, JournalDetail JournalDetail)
        {
            var success = await _accountService.CreditAccountAsync(JournalDetail.AccountId, JournalDetail.Amount);

            if (success)
            {
                await _ledgerService.AppendAsync(new LedgerEntry
                {
                    TransactionId = TransactionId,
                    TransactionLegId = JournalDetail.TransactionLegId,
                    Amount = JournalDetail.Amount,
                    AccountId = JournalDetail.AccountId,
                    Side = "Credit",
                    Timestamp = DateTime.UtcNow
                });
            }
            else
            {
                await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.TransactionFailed(TransactionId));
            }
        }

        [Activity]
        public async Task DebitAccountAsync(Guid TransactionId, JournalDetail JournalDetail)
        {
            var success = await _accountService.DebitAccountAsync(JournalDetail.AccountId, JournalDetail.Amount);

            if (success)
            {
                await _ledgerService.AppendAsync(new LedgerEntry
                {
                    TransactionId = TransactionId,
                    TransactionLegId = JournalDetail.TransactionLegId,
                    Amount = JournalDetail.Amount,
                    AccountId = JournalDetail.AccountId,
                    Side = "Debit",
                    Timestamp = DateTime.UtcNow
                });
            }
            else
            {
                await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.TransactionFailed(TransactionId));
            }
        }

        [Activity]
        public async Task ProcessTransactionAsync(Guid TransactionId)
        {
            JournalEntry transaction = await _journalService.GetTransactionAsync(TransactionId);

            foreach (var entry in transaction.JournalDetails.Where(j => j.Side == "Debit"))
            {
                await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.DebitAccountAsync(TransactionId, entry));
            }

            foreach (var entry in transaction.JournalDetails.Where(j => j.Side == "Credit"))
            {
                await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.CreditAccountAsync(TransactionId, entry));
            }

            await UpdateTransactionStatusAsync(TransactionId, State.Processed);
            await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.TransactionProcessed(TransactionId));
        }

        [Activity]
        public async Task UpdateTransactionStatusAsync(Guid transactionId, State state)
        {
            await _journalService.UpdateJournalStateAsync(transactionId, state.ToString());
        }
    }
}
