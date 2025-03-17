namespace DEAT.StateMachine
{
    //Commands
    public record StartTransaction(Guid TransactionId, System.UInt128 DebitAccountId, System.UInt128 CreditAccountId, System.UInt128 Amount);
    public record DebitAccount(Guid TransactionId, System.UInt128 AccountId, System.UInt128 Amount);
    public record CreditAccount(Guid TransactionId, System.UInt128 AccountId, System.UInt128 Amount);

    //Events - messages that can trigger state transitions. They must be correlated to a specific saga instance
    public record TransactionSucceeded(Guid TransactionId);
    public record TransactionFailed(Guid TransactionId);
    public record TransactionStatusRequested(Guid TransactionId, TransactionState TransactionState);
}
