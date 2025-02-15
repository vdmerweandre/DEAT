using Microsoft.AspNetCore.Mvc;
using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine;

namespace DEAT.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JournalController(
        IMessageService messageService,
        IJournalService journalService,
        ILogger<JournalController> logger) : ControllerBase
    {
        [HttpGet(Name = "GetAllTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<JournalEntry>> GetAllTransactions()
        {
            return await journalService.GetJournalEntriesAsync();
        }

        [HttpPost(Name = "CreateTransaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Guid> CreateTransaction([FromBody] JournalEntry transaction)
        {
            Guid transactionId = await journalService.CreateJournalEntryAsync(transaction);

            // Publish the StartTransaction event
            await messageService.SendCommand(new TransactionCreated(
                transactionId,
                transaction
            ));

            return transactionId;
        }

        [HttpPut("{transactionId:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Guid> ApproveTransaction([FromRoute] Guid transactionId)
        {
            bool success = await journalService.ApproveTransactionAsync(transactionId);

            if (success)
            {
                // Publish the Approval event
                await messageService.SendCommand(new TransactionApproved(transactionId));
            }

            return transactionId;
        }

        [HttpPut("{transactionId:guid}/legs/{transactionLegId:guid}/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Guid> ConfirmTransactionLeg([FromRoute] Guid transactionId, [FromRoute] Guid transactionLegId)
        {
            bool success = await journalService.ConfirmTransactionLegAsync(transactionId, transactionLegId);

            if (success)
            {
                // Publish the StartTransaction event
                await messageService.SendCommand(new TransactionLegConfirmed(transactionId, transactionLegId));
            }
           
            return transactionId;
        }

        [HttpPut("{transactionId:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Guid> CancelTransaction([FromRoute] Guid transactionId)
        {
            bool success = await journalService.CancelTransactionAsync(transactionId);

            if (success)
            {
                // Publish the StartTransaction event
                await messageService.SendCommand(new TransactionCancelled(transactionId));
            }

            return transactionId;
        }

        [HttpPut("{transactionId:guid}/retry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Guid> RetryTransaction([FromRoute] Guid transactionId)
        {
            bool success = await journalService.RetryTransactionAsync(transactionId);

            if (success)
            {
                // Publish the StartTransaction event
                await messageService.SendCommand(new TransactionRetried(transactionId));
            }

            return transactionId;
        }
    }
}
