using DEAT.Data.Models.Dtos;
using MassTransit;

namespace DEAT.WebAPI.Services.Statemachine.Observers
{
    public class AuditStateObserver<TInstance> : IStateObserver<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        private readonly List<StateChangeLog> _stateChangeLogs = new();

        public IReadOnlyList<StateChangeLog> StateChangeLogs => _stateChangeLogs.AsReadOnly();

        //public Task StateChanged(BehaviorContext<TransactionStateMachineInstance> context, State currentState, State previousState)
        //{
        //    _stateChangeLogs.Add(new StateChangeLog()
        //    {
        //        CorrelationId = context.Saga.CorrelationId,
        //        Version = context.Saga.Version,
        //        TransactionId = context.Saga.TransactionId,
        //        CurrentState = currentState.Name,
        //        PreviousState = previousState.Name,
        //        Timestamp = DateTime.UtcNow
        //    });

        //    return Task.CompletedTask;
        //}

        public Task StateChanged(BehaviorContext<TInstance> context, State currentState, State previousState)
        {
            _stateChangeLogs.Add(new StateChangeLog()
            {
                CorrelationId = context.Saga.CorrelationId,
                CurrentState = currentState.Name,
                PreviousState = previousState?.Name ?? string.Empty,
                Timestamp = DateTime.UtcNow
            });

            return Task.CompletedTask;
        }
    }
}
