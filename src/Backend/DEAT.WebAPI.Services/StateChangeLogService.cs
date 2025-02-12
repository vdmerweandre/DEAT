using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Statemachine;
using DEAT.WebAPI.Services.Statemachine.Observers;
using System.Collections.Concurrent;

namespace DEAT.WebAPI.Services
{
    public class StateChangeLogService(
        AuditStateObserver<TransactionStateMachineInstance> Observer,
        AuditEventObserver<TransactionStateMachineInstance> EventObserver)
    {
        public IReadOnlyList<StateChangeLog> GetStateChanges() => Observer.StateChangeLogs;
        public ConcurrentBag<EventLog> GetEventLogs() => EventObserver.EventLogs;
    }
}
