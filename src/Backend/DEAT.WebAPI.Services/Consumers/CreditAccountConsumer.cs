using DEAT.WebAPI.Services.Contracts;
using DEAT.WebAPI.Services.Statemachine;
using MassTransit;

namespace DEAT.WebAPI.Services.Consumers
{
    public class CreditAccountConsumer(
        IAccountService accountService,
        ILedgerService ledgerService) : IConsumer<CreditAccount>
    {
        public async Task Consume(ConsumeContext<CreditAccount> context)
        {
            var success = await accountService.CreditAccountAsync(context.Message.JournalDetail.AccountId, context.Message.JournalDetail.Amount);

            if (success)
            {
                await ledgerService.AppendAsync(new Data.Models.Dtos.LedgerEntry
                {
                    TransactionId = context.Message.TransactionId,
                    TransactionLegId = context.Message.JournalDetail.TransactionLegId,
                    Amount = context.Message.JournalDetail.Amount,
                    AccountId = context.Message.JournalDetail.AccountId,
                    Side = "Credit",
                    Timestamp = DateTime.UtcNow
                });
            }
            else
            {
                await context.Publish(new TransactionFailed(context.Message.TransactionId));
            }
        }
    }

}
