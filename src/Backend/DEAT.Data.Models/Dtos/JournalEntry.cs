namespace DEAT.Data.Models.Dtos
{
    public class JournalDetail
    {
        public Guid TransactionLegId { get; set; }
        public System.UInt128? AccountId { get; set; }
        public System.UInt128? Amount { get; set; }
        public string? State { get; set; }
        public string? Side { get; set; }
        public string? Category { get; set; }
    }

    public class JournalEntry
    {
        public Guid TransactionId { get; set; }
        public string? Reference { get; set; }
        public JournalDetail[] JournalDetails { get; set; } = new JournalDetail[0];
        public System.UInt128 NetBalance
        {
            get
            {
                System.UInt128 liabilitiesBalance = SumByCategory("Liabilities");
                System.UInt128 incomeBalance = SumByCategory("Income");
                System.UInt128 equityBalance = SumByCategory("Equity");
                System.UInt128 assetsBalance = SumByCategory("Assets");
                System.UInt128 expensesBalance = SumByCategory("Expenses");

                return liabilitiesBalance + incomeBalance + equityBalance - assetsBalance - expensesBalance;
            }
        }

        private System.UInt128 SumByCategory(string category)
        {
            var debits = JournalDetails
                .Where(l => l.Category == category && l.Side == "Debit" && l.Amount.HasValue)
                .Aggregate(System.UInt128.Zero, (acc, l) => acc + l.Amount.Value);

            var credits = JournalDetails
                .Where(l => l.Category == category && l.Side == "Credit" && l.Amount.HasValue)
                .Aggregate(System.UInt128.Zero, (acc, l) => acc + l.Amount.Value);

            return debits - credits;
        }

        public System.UInt128 DebitAmount => JournalDetails
            .Where(l => l.Side == "Debit" && l.Amount.HasValue)
            .Aggregate(System.UInt128.Zero, (acc, l) => acc + l.Amount.Value);

        public System.UInt128 CreditAmount => JournalDetails
            .Where(l => l.Side == "Credit" && l.Amount.HasValue)
            .Aggregate(System.UInt128.Zero, (acc, l) => acc + l.Amount.Value);

        public string? State { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Version { get; set; }
    }
}
