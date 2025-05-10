namespace DEAT.Data.Models.Dtos
{
    public class LedgerEntry
    {
        public Guid TransactionId { get; set; }
        public Guid TransactionLegId { get; set; }
        public System.UInt128? AccountId { get; set; }
        public System.UInt128? Amount { get; set; }
        public string Side { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
