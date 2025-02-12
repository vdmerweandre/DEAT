using Microsoft.AspNetCore.Mvc;
using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine;

namespace DEAT.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController(
        IMessageService messageService,
        ITransactionService transactionService,
        ILogger<TransactionsController> logger) : ControllerBase
    {
        [HttpGet(Name = "GetAllTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<TransactionDto>> GetAllTransactions()
        {
            return await transactionService.GetTransactionsAsync();
        }

        [HttpPost(Name = "CreateTransaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Guid> CreateTransaction(TransactionDto transaction)
        {
            Guid transactionId = await transactionService.CreateTransactionAsync(transaction);

            // Publish the StartTransaction event
            await messageService.SendCommand(new TransactionCreated(
                transactionId,
                transaction.DebitAccountId,
                transaction.TransactionLegs.Select(l => new TransactionLeg
                {
                    TransactionLegId = l.TransactionLegId,
                    CreditAccountId = l.CreditAccountId,
                    Amount = l.Amount,
                    State = l.State
                }).ToArray(),
                transaction.Amount
            ));

            return transactionId;
        }

        [HttpPut("{transactionId:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Guid> ApproveTransaction([FromRoute] Guid transactionId)
        {
            bool success = await transactionService.ApproveTransactionAsync(transactionId);

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
            bool success = await transactionService.ConfirmTransactionLegAsync(transactionId, transactionLegId);

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
            bool success = await transactionService.CancelTransactionAsync(transactionId);

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
            bool success = await transactionService.RetryTransactionAsync(transactionId);

            if (success)
            {
                // Publish the StartTransaction event
                await messageService.SendCommand(new TransactionRetried(transactionId));
            }

            return transactionId;
        }
    }
}
