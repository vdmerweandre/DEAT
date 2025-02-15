
namespace DEAT.Data.Models.Dtos
{
    public class LedgerEntry
    {
        public Guid TransactionId { get; set; }
        public Guid TransactionLegId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Side { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
