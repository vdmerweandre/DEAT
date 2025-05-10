namespace DEAT.Data.Models.Dtos
{
    public class Account
    {
        public System.UInt128? AccountId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public System.UInt128? Balance
        {
            get
            {
                if (!Debit.HasValue || !Credit.HasValue)
                    return null;
                return Debit.Value - Credit.Value;
            }
        }

        public System.UInt128? Debit { get; set; }
        public System.UInt128? Credit { get; set; }
    }
}
