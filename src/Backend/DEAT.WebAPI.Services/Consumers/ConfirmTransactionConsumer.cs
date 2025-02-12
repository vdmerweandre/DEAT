using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine;
using MassTransit;

namespace DEAT.WebAPI.Services.Consumers
{
    public class ConfirmTransactionConsumer(ITransactionService transactionService) : IConsumer<ConfirmTransaction>
    {
        public async Task Consume(ConsumeContext<ConfirmTransaction> context)
        {
            var success = await transactionService.ConfirmTransactionAsync(context.Message.TransactionId);

            if (success)
            {
                await context.Publish(new TransactionSucceeded(context.Message.TransactionId));
            }
        }
    }

}
