using MassTransit;

namespace DEAT.WebAPI.Services.Statemachine
{
    public class TransactionLeg
    {
        public int Version { get; set; }
        public Guid TransactionLegId { get; set; }
        public Guid CreditAccountId { get; set; }
        public decimal Amount { get; set; }
        public string State { get; set; }
    }

    public class TransactionStateMachineInstance : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public Guid TransactionId { get; set; }
        public Guid DebitAccountId { get; set; }
        public TransactionLeg[] TransactionLegs { get; set; }
        public decimal Amount { get; set; }
        public string State { get; set; }
    }
}
