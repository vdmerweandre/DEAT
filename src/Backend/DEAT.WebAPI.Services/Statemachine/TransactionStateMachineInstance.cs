using DEAT.Data.Models.Dtos;
using MassTransit;

namespace DEAT.WebAPI.Services.Statemachine
{
    public class TransactionStateMachineInstance : JournalEntry, SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
    }
}
