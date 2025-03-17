using TigerBeetle;

namespace DEAT.WebAPI.TigerBeetleServices;

public static class AccountDefinitions
{
    public static readonly (UInt128 Id, string Name, AccountFlags Flags, ushort Code)[] Accounts = new[]
    {
        // Assets
        (Id: UInt128.Parse("101"), Name: "Bank", Flags: AccountFlags.DebitsMustNotExceedCredits, Code: (ushort)1),
        (Id: UInt128.Parse("102"), Name: "Hot wallet - BTC", Flags: AccountFlags.DebitsMustNotExceedCredits, Code: (ushort)1),
        (Id: UInt128.Parse("103"), Name: "Hot wallet - SOL", Flags: AccountFlags.DebitsMustNotExceedCredits, Code: (ushort)1),

        // Liabilities
        (Id: UInt128.Parse("201"), Name: "Accounts Payable", Flags: AccountFlags.CreditsMustNotExceedDebits, Code: (ushort)2),
        (Id: UInt128.Parse("202"), Name: "Liquidity Provider", Flags: AccountFlags.CreditsMustNotExceedDebits, Code: (ushort)2),

        // Equity
        (Id: UInt128.Parse("301"), Name: "Capital", Flags: AccountFlags.CreditsMustNotExceedDebits, Code: (ushort)3),

        // Income
        (Id: UInt128.Parse("401"), Name: "Accounts Receivable", Flags: AccountFlags.CreditsMustNotExceedDebits, Code: (ushort)4),
        (Id: UInt128.Parse("402"), Name: "Withdrawal Fees", Flags: AccountFlags.CreditsMustNotExceedDebits, Code: (ushort)4),
        (Id: UInt128.Parse("403"), Name: "Deposit Fees", Flags: AccountFlags.CreditsMustNotExceedDebits, Code: (ushort)4),
        (Id: UInt128.Parse("404"), Name: "Brokerage Fees", Flags: AccountFlags.CreditsMustNotExceedDebits, Code: (ushort)4),
        (Id: UInt128.Parse("405"), Name: "Commission", Flags: AccountFlags.CreditsMustNotExceedDebits, Code: (ushort)4),

        // Expenses
        (Id: UInt128.Parse("501"), Name: "Network Fees", Flags: AccountFlags.DebitsMustNotExceedCredits, Code: (ushort)5),
        (Id: UInt128.Parse("502"), Name: "Merchant Fees", Flags: AccountFlags.DebitsMustNotExceedCredits, Code: (ushort)5)
    };

    public static TigerBeetle.Account CreateAccount(UInt128 id, string name, AccountFlags flags, ushort code)
    {
        return new TigerBeetle.Account
        {
            Id = id,
            UserData128 = UInt128.Zero,
            UserData64 = 0,
            Timestamp = 0,
            Flags = flags,
            Ledger = 1,
            Code = code
        };
    }
} 