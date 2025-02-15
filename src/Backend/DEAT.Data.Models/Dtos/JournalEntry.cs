namespace DEAT.Data.Models.Dtos
{
    public class JournalDetail
    {
        public Guid TransactionLegId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? State { get; set; }
        public string? Side { get; set; }
        public string? Category { get; set; }
    }

    public class JournalEntry
    {
        public Guid TransactionId { get; set; }
        public string? Reference { get; set; }
        public JournalDetail[] JournalDetails { get; set; } = new JournalDetail[0];
        public decimal NetBalance =>
            (JournalDetails.Where(l => l.Category == "Liabilities" && l.Side == "Debit").Sum(l => l.Amount) - JournalDetails.Where(l => l.Category == "Liabilities" && l.Side == "Credit").Sum(l => l.Amount)) +
            (JournalDetails.Where(l => l.Category == "Income" && l.Side == "Debit").Sum(l => l.Amount) - JournalDetails.Where(l => l.Category == "Income" && l.Side == "Credit").Sum(l => l.Amount)) +
            (JournalDetails.Where(l => l.Category == "Equity" && l.Side == "Debit").Sum(l => l.Amount) - JournalDetails.Where(l => l.Category == "Equity" && l.Side == "Credit").Sum(l => l.Amount)) -
            (JournalDetails.Where(l => l.Category == "Assets" && l.Side == "Debit").Sum(l => l.Amount) - JournalDetails.Where(l => l.Category == "Assets" && l.Side == "Credit").Sum(l => l.Amount)) -
            (JournalDetails.Where(l => l.Category == "Expenses" && l.Side == "Debit").Sum(l => l.Amount) - JournalDetails.Where(l => l.Category == "Expenses" && l.Side == "Credit").Sum(l => l.Amount));

        public decimal DebitAmount => JournalDetails.Where(l => l.Side == "Debit").Sum(l => l.Amount);
        public decimal CreditAmount => JournalDetails.Where(l => l.Side == "Credit").Sum(l => l.Amount);
        public string? State { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Version { get; set; }
    }

}
