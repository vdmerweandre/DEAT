
namespace DEAT.Data.Models.Dtos
{
    public class EventLog
    {
        public Guid SagaId { get; set; }
        public string EventName { get; set; }
        public string Status { get; set; }
        public string? ExceptionMessage { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
