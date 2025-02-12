using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine;
using MassTransit;

namespace DEAT.WebAPI.Services.Consumers
{
    public class DebitAccountConsumer : IConsumer<ProcessTransaction>
    {
        private readonly IAccountService _accountService;

        public DebitAccountConsumer(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task Consume(ConsumeContext<ProcessTransaction> context)
        {
            var success = await _accountService.DebitAccountAsync(context.Message.DebitAccountId, context.Message.Amount);

            if (success)
            {
                foreach (var leg in context.Message.TransactionLegs)
                {
                    await context.Publish(new CreditAccount(context.Message.TransactionId, leg.CreditAccountId, leg.Amount));
                }

                await context.Publish(new TransactionProcessed(context.Message.TransactionId));
            }
            else
            {
                await context.Publish(new TransactionFailed(context.Message.TransactionId));
            }
        }
    }

}
