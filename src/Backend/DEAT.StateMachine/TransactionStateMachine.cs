using MassTransit;

namespace DEAT.StateMachine
{
    public class TransactionState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public Guid TransactionId { get; set; }
        public Guid DebitAccountId { get; set; }
        public Guid CreditAccountId { get; set; }
        public decimal Amount { get; set; }
        public string State { get; set; }
    }

    public class TransactionStateMachine : MassTransitStateMachine<TransactionState>
    {
        public State Pending { get; private set; }
        public State Completed { get; private set; }
        public State Failed { get; private set; }

        public Event<StartTransaction> StartTransaction { get; private set; }
        public Event<TransactionSucceeded> TransactionSucceeded { get; private set; }
        public Event<TransactionFailed> TransactionFailed { get; private set; }
        public Event<TransactionStatusRequested> TransactionStatusRequested { get; private set; }

        public TransactionStateMachine()
        {
            InstanceState(x => x.State);

            Event(() => StartTransaction, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionSucceeded, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionFailed, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionStatusRequested, x => x.CorrelateById(m => m.Message.TransactionId));

            Initially(
                When(StartTransaction)
                    .Then(context =>
                    {
                        context.Saga.TransactionId = context.Message.TransactionId;
                        context.Saga.DebitAccountId = context.Message.DebitAccountId;
                        context.Saga.CreditAccountId = context.Message.CreditAccountId;
                        context.Saga.Amount = context.Message.Amount;
                    })
                    .TransitionTo(Pending)
                    .Publish(context => new DebitAccount(
                        context.Saga.TransactionId, 
                        context.Saga.DebitAccountId, 
                        context.Saga.Amount)
                    )
            );

            During(Pending,
                When(TransactionSucceeded)
                    .TransitionTo(Completed),
                When(TransactionFailed)
                    .TransitionTo(Failed)
            );

            DuringAny(
                When(TransactionStatusRequested)
                    .RespondAsync(x => x.Init<TransactionStatusRequested>(new { x.Saga })
                    )
            );
        }
    }

}
