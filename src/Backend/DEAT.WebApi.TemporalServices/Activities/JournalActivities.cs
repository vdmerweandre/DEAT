using DEAT.Data.Models.Dtos;
using DEAT.WebApi.TemporalServices.Contract;
using DEAT.WebApi.TemporalServices.Models;
using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.TigerBeetleServices;
using Temporalio.Activities;
using Microsoft.Extensions.Logging;

namespace DEAT.WebApi.TemporalServices.Activities
{
    public class JournalActivities : IJournalActivities
    {
        private readonly ILogger<JournalActivities> _logger;
        private readonly IJournalService _journalService;
        private readonly DEAT.WebAPI.TigerBeetleServices.IAccountService _accountService;
        private readonly ILedgerService _ledgerService;
        private TemporalClientService _temporalClientService;

        public JournalActivities(
            ILogger<JournalActivities> logger,
            IJournalService transactionService,
            DEAT.WebAPI.TigerBeetleServices.IAccountService accountService,
            ILedgerService ledgerService,
            TemporalClientService temporalClientService)
        {
            _logger = logger;
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
            try
            {
                if (!JournalDetail.AccountId.HasValue || !JournalDetail.Amount.HasValue)
                {
                    _logger.LogError("AccountId or Amount is null for transaction {TransactionId}", TransactionId);
                    await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.TransactionFailed(TransactionId));
                    return;
                }

                _logger.LogInformation("Crediting account {AccountId} with amount {Amount}", JournalDetail.AccountId, JournalDetail.Amount);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crediting account {AccountId}", JournalDetail.AccountId);
                await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.TransactionFailed(TransactionId));
            }
        }

        [Activity]
        public async Task DebitAccountAsync(Guid TransactionId, JournalDetail JournalDetail)
        {
            try
            {
                if (!JournalDetail.AccountId.HasValue || !JournalDetail.Amount.HasValue)
                {
                    _logger.LogError("AccountId or Amount is null for transaction {TransactionId}", TransactionId);
                    await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.TransactionFailed(TransactionId));
                    return;
                }

                _logger.LogInformation("Debiting account {AccountId} with amount {Amount}", JournalDetail.AccountId, JournalDetail.Amount);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error debiting account {AccountId}", JournalDetail.AccountId);
                await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.TransactionFailed(TransactionId));
            }
        }

        [Activity]
        public async Task ProcessTransactionAsync(Guid TransactionId)
        {
            var transaction = await _journalService.GetTransactionAsync(TransactionId);
            if (transaction == null)
            {
                _logger.LogError("Transaction not found: {TransactionId}", TransactionId);
                return;
            }

            foreach (var entry in transaction.JournalDetails.Where(j => j.Side == "Debit" && j.AccountId.HasValue && j.Amount.HasValue))
            {
                await _temporalClientService.SendWfSignalAsync(TransactionId, wf => wf.DebitAccountAsync(TransactionId, entry));
            }

            foreach (var entry in transaction.JournalDetails.Where(j => j.Side == "Credit" && j.AccountId.HasValue && j.Amount.HasValue))
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

        public async Task<JournalEntry> GetJournalEntryAsync(Guid transactionId)
        {
            var journalEntry = await _journalService.GetTransactionAsync(transactionId);

            if (journalEntry == null)
                throw new ArgumentException($"Journal entry not found for transaction {transactionId}");

            return journalEntry;
        }
    }
}
