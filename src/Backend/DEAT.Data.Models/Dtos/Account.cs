namespace DEAT.Data.Models.Dtos
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public string Category { get; set; }
        public string AccountName { get; set; }
        public decimal Balance
        {
            get
            {
                return Debit - Credit;
}
        }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
