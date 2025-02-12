using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine;
using MassTransit;

namespace DEAT.WebAPI.Services.Consumers
{
    public class CreditAccountConsumer : IConsumer<CreditAccount>
    {
        private readonly IAccountService _accountService;

        public CreditAccountConsumer(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task Consume(ConsumeContext<CreditAccount> context)
        {
            var success = await _accountService.CreditAccountAsync(context.Message.AccountId, context.Message.Amount);

            if (!success)
            {
                await context.Publish(new TransactionFailed(context.Message.TransactionId));
            }
            //if (success)
            //{
            //    await context.Publish(new TransactionSucceeded(context.Message.TransactionId));
            //}
            //else
            //{
            //    await context.Publish(new TransactionFailed(context.Message.TransactionId));
            //}
        }
    }

}
