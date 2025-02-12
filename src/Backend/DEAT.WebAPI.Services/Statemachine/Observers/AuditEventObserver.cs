using DEAT.Data.Models.Dtos;
using MassTransit;
using System.Collections.Concurrent;

namespace DEAT.WebAPI.Services.Statemachine.Observers
{
    public class AuditEventObserver<TInstance> : IEventObserver<TInstance>
    where TInstance : class, SagaStateMachineInstance
    {
        private readonly ConcurrentBag<EventLog> _eventLogs = new();

        public ConcurrentBag<EventLog> EventLogs => _eventLogs;

        public Task PreExecute(BehaviorContext<TInstance> context)
        {
            if (context?.Event == null || context?.Saga == null)
                return Task.CompletedTask;

            _eventLogs.Add(new EventLog
            {
                SagaId = context.Saga.CorrelationId,
                EventName = context.Event.Name,
                Timestamp = DateTime.UtcNow,
                Status = "PreExecute"
            });

            return Task.CompletedTask;
        }

        public Task PostExecute(BehaviorContext<TInstance> context)
        {
            if (context?.Event == null || context?.Saga == null)
                return Task.CompletedTask;

            _eventLogs.Add(new EventLog
            {
                SagaId = context.Saga.CorrelationId,
                EventName = context.Event.Name,
                Timestamp = DateTime.UtcNow,
                Status = "PostExecute"
            });

            return Task.CompletedTask;
        }

        public Task ExecuteFault(BehaviorContext<TInstance> context, Exception exception)
        {
            if (context?.Event == null || context?.Saga == null)
                return Task.CompletedTask;

            _eventLogs.Add(new EventLog
            {
                SagaId = context.Saga.CorrelationId,
                EventName = context.Event.Name,
                Timestamp = DateTime.UtcNow,
                Status = "ExecuteFault",
                ExceptionMessage = exception.Message
            });

            return Task.CompletedTask;
        }

        public Task PreExecute<T>(BehaviorContext<TInstance, T> context) where T : class
        {
            if (context?.Event == null || context?.Saga == null)
                return Task.CompletedTask;

            _eventLogs.Add(new EventLog
            {
                SagaId = context.Saga.CorrelationId,
                EventName = context.Event.Name,
                Timestamp = DateTime.UtcNow,
                Status = "PreExecute"
            });

            return Task.CompletedTask;
        }

        public Task PostExecute<T>(BehaviorContext<TInstance, T> context) where T : class
        {
            if (context?.Event == null || context?.Saga == null)
                return Task.CompletedTask;

            _eventLogs.Add(new EventLog
            {
                SagaId = context.Saga.CorrelationId,
                EventName = context.Event.Name,
                Timestamp = DateTime.UtcNow,
                Status = "PostExecute"
            });

            return Task.CompletedTask;
        }

        public Task ExecuteFault<T>(BehaviorContext<TInstance, T> context, Exception exception) where T : class
        {
            if (context?.Event == null || context?.Saga == null)
                return Task.CompletedTask;

            _eventLogs.Add(new EventLog
            {
                SagaId = context.Saga.CorrelationId,
                EventName = context.Event.Name,
                Timestamp = DateTime.UtcNow,
                Status = "ExecuteFault",
                ExceptionMessage = exception.Message
            });

            return Task.CompletedTask;
        }
    }
}
