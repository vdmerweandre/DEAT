using System.Security.Cryptography;
using DEAT.Data.Models.Dtos;
using TigerBeetle;

namespace DEAT.WebAPI.TigerBeetleServices;

#nullable enable

public static class AccountMapper
{
    private static readonly Dictionary<string, UInt16> CategoryCodes = new()
    {
        { "Assets", 1 },
        { "Liabilities", 2 },
        { "Equity", 3 },
        { "Income", 4 },
        { "Expenses", 5 }
    };

    public static IEnumerable<string> GetAllCategories()
    {
        return CategoryCodes.Keys;
    }

    public static UInt16 GetCategoryCode(string category)
    {
        return CategoryCodes.GetValueOrDefault(category, (UInt16)0);
    }

    public static TigerBeetle.Account ToTigerBeetleAccount(this DEAT.Data.Models.Dtos.Account account)
    {
        // If account already has an ID, use it
        if (account.AccountId.HasValue)
        {
            return CreateTigerBeetleAccount(account.AccountId.Value, account.Category);
        }

        // Generate a deterministic ID based on category and name
        var accountId = GenerateAccountId(account.Category, account.AccountName);
        return CreateTigerBeetleAccount(accountId, account.Category);
    }

    public static DEAT.Data.Models.Dtos.Account ToDtoAccount(this TigerBeetle.Account tbAccount, string? accountName = null)
    {
        ArgumentNullException.ThrowIfNull(tbAccount);

        var category = GetCategoryFromCode(tbAccount.Code);
        var finalAccountName = accountName ?? $"{category}_{tbAccount.Id}";

        return new DEAT.Data.Models.Dtos.Account
        {
            AccountId = tbAccount.Id,
            Category = category,
            AccountName = finalAccountName,
            Debit = tbAccount.DebitsPosted,
            Credit = tbAccount.CreditsPosted
        };
    }

    private static UInt128 GenerateAccountId(string category, string accountName)
    {
        // Create a deterministic hash from category and account name
        using var sha256 = SHA256.Create();
        var input = $"{category}:{accountName}";
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        
        // Take the first 16 bytes for UInt128
        var bytes = new byte[16];
        Array.Copy(hash, bytes, 16);
        
        // Ensure the first byte matches the category code for easy identification
        bytes[0] = (byte)CategoryCodes.GetValueOrDefault(category, (UInt16)0);
        
        return new UInt128(BitConverter.ToUInt64(bytes, 8), BitConverter.ToUInt64(bytes, 0));
    }

    private static TigerBeetle.Account CreateTigerBeetleAccount(UInt128 id, string category)
    {
        return new TigerBeetle.Account
        {
            Id = id,
            UserData128 = UInt128.Zero,
            UserData64 = 0,
            Timestamp = 0,
            Flags = GetAccountFlags(category),
            Ledger = 1,
            Code = CategoryCodes.GetValueOrDefault(category, (UInt16)0)
        };
    }

    private static AccountFlags GetAccountFlags(string category)
    {
        return category.ToLower() switch
        {
            "assets" => AccountFlags.DebitsMustNotExceedCredits,
            "expenses" => AccountFlags.DebitsMustNotExceedCredits,
            "liabilities" => AccountFlags.CreditsMustNotExceedDebits,
            "equity" => AccountFlags.CreditsMustNotExceedDebits,
            "income" => AccountFlags.CreditsMustNotExceedDebits,
            _ => AccountFlags.None
        };
    }

    private static string GetCategoryFromCode(UInt16 code)
    {
        return CategoryCodes.FirstOrDefault(x => x.Value == code).Key ?? "Unknown";
    }
} 