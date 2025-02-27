using Microsoft.AspNetCore.Mvc;
using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine;
using System.Transactions;

namespace DEAT.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JournalController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IJournalService _journalService;
        private readonly ILogger<JournalController> _logger;
        private readonly TemporalClientService _temporalClientService;

        private static bool _useTemporalService = true;

        public JournalController(
            IMessageService messageService,
            IJournalService journalService,
            TemporalClientService temporalClientService,
            ILogger<JournalController> logger)
        {
            _messageService = messageService;
            _journalService = journalService;
            _logger = logger;
            _temporalClientService = temporalClientService;
        }

        [HttpGet(Name = "GetAllTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<JournalEntry>> GetAllTransactions()
        {
            return await _journalService.GetJournalEntriesAsync();
        }

        [HttpPost(Name = "CreateTransaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Guid> CreateTransaction([FromBody] JournalEntry transaction)
        {
            // Generate a new Transaction Id
            Guid transactionId = Guid.NewGuid();

            if (_useTemporalService)
            {
                await _temporalClientService.StartWorkflowAsync(transactionId, transaction);
            }
            else
            {
                await _journalService.CreateJournalEntryAsync(transactionId, transaction);

                await _messageService.SendCommand(new TransactionCreated(
                    transactionId,
                    transaction
                ));
            }

            return transactionId;
        }

        [HttpPut("{transactionId:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Guid> ApproveTransaction([FromRoute] Guid transactionId)
        {
            if (_useTemporalService)
            {
                var transaction = await _journalService.GetTransactionAsync(transactionId);
                await _temporalClientService.SendWfSignalAsync(transactionId, wf => wf.ApproveTransactionAsync(transactionId));
            }
            else
            {
                bool success = await _journalService.ApproveTransactionAsync(transactionId);
                if (success)
                {
                    // Publish the Approval event
                    await _messageService.SendCommand(new TransactionApproved(transactionId));
                }
            }

            return transactionId;
        }

        [HttpPut("{transactionId:guid}/legs/{transactionLegId:guid}/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Guid> ConfirmTransactionLeg([FromRoute] Guid transactionId, [FromRoute] Guid transactionLegId)
        {
            if (_useTemporalService)
            {
                await _temporalClientService.SendWfSignalAsync(transactionId, wf => wf.ConfirmTransactionLegAsync(transactionId, transactionLegId));
            }
            else
            {
                bool success = await _journalService.ConfirmTransactionLegAsync(transactionId, transactionLegId);
                if (success)
                {
                    // Publish the StartTransaction event
                    await _messageService.SendCommand(new TransactionLegConfirmed(transactionId, transactionLegId));
                }
            }

            return transactionId;
        }

        [HttpPut("{transactionId:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Guid> CancelTransaction([FromRoute] Guid transactionId)
        {
            if (_useTemporalService)
            {
                await _temporalClientService.SendWfSignalAsync(transactionId, wf => wf.CancelTransactionAsync(transactionId));
            }
            else
            {
                bool success = await _journalService.CancelTransactionAsync(transactionId);
                if (success)
                {
                    // Publish the StartTransaction event
                    await _messageService.SendCommand(new TransactionCancelled(transactionId));
                }
            }

            return transactionId;
        }

        [HttpPut("{transactionId:guid}/retry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Guid> RetryTransaction([FromRoute] Guid transactionId)
        {
            bool success = await _journalService.RetryTransactionAsync(transactionId);

            if (success)
            {
                // Publish the StartTransaction event
                await _messageService.SendCommand(new TransactionRetried(transactionId));
            }

            return transactionId;
        }
    }
}
