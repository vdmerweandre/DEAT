using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine;
using MassTransit;

namespace DEAT.WebAPI.Services.Consumers
{
    public class UpdateTransactionStatusConsumer(IJournalService transactionService) : IConsumer<UpdateTransactionStatus>
    {
        public async Task Consume(ConsumeContext<UpdateTransactionStatus> context)
        {
            var success = await transactionService.UpdateJournalStateAsync(context.Message.TransactionId, context.Message.status);
        }
    }

}
