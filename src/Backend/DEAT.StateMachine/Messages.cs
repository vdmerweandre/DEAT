
namespace DEAT.StateMachine
{
    //Commands
    public record StartTransaction(Guid TransactionId, Guid DebitAccountId, Guid CreditAccountId, decimal Amount);
    public record DebitAccount(Guid TransactionId, Guid AccountId, decimal Amount);
    public record CreditAccount(Guid TransactionId, Guid AccountId, decimal Amount);

    //Events - messages that can trigger state transitions. They must be correlated to a specific saga instance
    public record TransactionSucceeded(Guid TransactionId);
    public record TransactionFailed(Guid TransactionId);
    public record TransactionStatusRequested(Guid TransactionId, TransactionState TransactionState);
}
