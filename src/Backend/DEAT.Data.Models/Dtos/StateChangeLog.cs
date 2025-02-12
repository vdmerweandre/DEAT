
namespace DEAT.Data.Models.Dtos
{
    public class StateChangeLog
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public Guid TransactionId { get; set; }
        public string CurrentState { get; set; }
        public string PreviousState { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
