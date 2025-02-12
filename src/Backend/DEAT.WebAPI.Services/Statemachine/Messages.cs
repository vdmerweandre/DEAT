namespace DEAT.WebAPI.Services.Statemachine
{
    //Commands
    //Internal
    public record ProcessTransaction(Guid TransactionId, Guid DebitAccountId, TransactionLeg[] TransactionLegs, decimal Amount);
    public record CreditAccount(Guid TransactionId, Guid AccountId, decimal Amount);
    public record ConfirmTransaction(Guid TransactionId);
    public record UpdateTransactionStatus(Guid TransactionId, string status);

    //Events - messages that can trigger state transitions. They must be correlated to a specific saga instance
    public record TransactionCreated(Guid TransactionId, Guid DebitAccountId, TransactionLeg[] TransactionLegs, decimal Amount);
    public record TransactionSucceeded(Guid TransactionId);
    public record TransactionApproved(Guid TransactionId);
    public record TransactionProcessed(Guid TransactionId);
    public record TransactionLegConfirmed(Guid TransactionId, Guid TransactionLegId);
    public record TransactionCancelled(Guid TransactionId);
    public record TransactionFailed(Guid TransactionId);
    public record TransactionRetried(Guid TransactionId);
    public record TransactionStatusRequested(Guid TransactionId, TransactionStateMachineInstance TransactionState);
}
