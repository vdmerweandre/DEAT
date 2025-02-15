using MassTransit;

namespace DEAT.WebAPI.Services.Statemachine
{
    public class TransactionStateMachine : MassTransitStateMachine<TransactionStateMachineInstance>
    {
        public State Created { get; private set; }
        public State Approved { get; private set; }
        public State Processed { get; private set; }
        public State Completed { get; private set; }
        public State Cancelled { get; private set; }
        public State Failed { get; private set; }

        public Event<TransactionCreated> TransactionCreated { get; private set; }
        public Event<TransactionSucceeded> TransactionSucceeded { get; private set; }
        public Event<TransactionApproved> TransactionApproved { get; private set; }
        public Event<TransactionProcessed> TransactionProcessed { get; private set; }
        public Event<TransactionLegConfirmed> TransactionLegConfirmed { get; private set; }
        public Event<TransactionFailed> TransactionFailed { get; private set; }
        public Event<TransactionRetried> TransactionRetried { get; private set; }
        public Event<TransactionCancelled> TransactionCancelled { get; private set; }
        public Event<TransactionStatusRequested> TransactionStatusRequested { get; private set; }

        public TransactionStateMachine()
        {
            InstanceState(x => x.State);

            Event(() => TransactionCreated, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionApproved, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionProcessed, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionLegConfirmed, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionFailed, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionRetried, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionCancelled, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionSucceeded, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => TransactionStatusRequested, x => x.CorrelateById(m => m.Message.TransactionId));

            Initially(
                When(TransactionCreated)
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.TransactionId;
                        context.Saga.TransactionId = context.Message.JournalEntry.TransactionId;
                        context.Saga.JournalDetails = context.Message.JournalEntry.JournalDetails;
                        context.Saga.Timestamp = context.Message.JournalEntry.Timestamp;
                        context.Saga.Reference = context.Message.JournalEntry.Reference;
                        context.Saga.State = context.Message.JournalEntry.State;
                    })
                    .TransitionTo(Created)
                    .Publish(context => new UpdateTransactionStatus(
                        context.Saga.TransactionId,
                        "Created")
                    )
            );

            During(Created,
                When(TransactionApproved)
                    .TransitionTo(Approved)
                    .Publish(context => new UpdateTransactionStatus(
                        context.Saga.CorrelationId,
                        "Approved")
                    )
                    .Publish(context => new ProcessTransaction(
                        context.Saga.TransactionId,
                        context.Saga.JournalDetails)
                    ),
                When(TransactionCancelled)
                    .TransitionTo(Cancelled)
                    .Publish(context => new UpdateTransactionStatus(
                        context.Saga.CorrelationId,
                        "Cancelled")
                    )
            );

            During(Approved,
                When(TransactionProcessed)
                    .TransitionTo(Processed)
                    .Publish(context => new UpdateTransactionStatus(
                        context.Saga.CorrelationId,
                        "Processed")
                    ),
                When(TransactionFailed)
                    .TransitionTo(Failed)
                    .Publish(context => new UpdateTransactionStatus(
                        context.Saga.CorrelationId,
                        "Failed")
                    )
            );

            During(Processed,
                //no state transition, keep trying to confirm transaction untill TransactionSucceeded event
                When(TransactionLegConfirmed)
                    .Publish(context => new ConfirmTransaction(
                        context.Saga.CorrelationId)
                    ),
                When(TransactionSucceeded)
                    .TransitionTo(Completed)
                    .Publish(context => new UpdateTransactionStatus(
                        context.Saga.CorrelationId,
                        "Success")
                    )
            );

            During(Failed,
                When(TransactionRetried)
                    .Then(context =>
                    {
                        context.Saga.Version++;
                    })
                    .TransitionTo(Created)
                    //automatically approve transaction based on retry initiated
                    .Publish(context => new TransactionApproved(
                        context.Saga.TransactionId
                    ))
            );

            DuringAny(
                When(TransactionStatusRequested)
                    .RespondAsync(x => x.Init<TransactionStatusRequested>(new { x.Saga })
                    )
            );
        }
    }

}
