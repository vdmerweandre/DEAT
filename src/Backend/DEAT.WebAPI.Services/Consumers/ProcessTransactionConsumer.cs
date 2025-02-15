using DEAT.WebAPI.Services.Statemachine;
using MassTransit;

namespace DEAT.WebAPI.Services.Consumers
{
    public class ProcessTransactionConsumer : IConsumer<ProcessTransaction>
    {
        public async Task Consume(ConsumeContext<ProcessTransaction> context)
        {
            foreach (var entry in context.Message.JournalDetails.Where(j => j.Side == "Debit"))
            {
                await context.Publish(new DebitAccount(context.Message.TransactionId, entry));
            }

            foreach (var entry in context.Message.JournalDetails.Where(j => j.Side == "Credit"))
            {
                await context.Publish(new CreditAccount(context.Message.TransactionId, entry));
            }

            await context.Publish(new TransactionProcessed(context.Message.TransactionId));
        }
    }

}
