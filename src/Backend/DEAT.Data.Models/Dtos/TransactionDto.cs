namespace DEAT.Data.Models.Dtos
{
    public class TransactionLegDto
    {
        public Guid TransactionLegId { get; set; }
        public Guid CreditAccountId { get; set; }
        public decimal Amount { get; set; }
        public string State { get; set; }
    }

    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid DebitAccountId { get; set; }
        public TransactionLegDto[] TransactionLegs { get; set; } = new TransactionLegDto[0];
        public decimal Amount { get; set; }
        public string? Status { get; set; }
    }

}
