using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using static MassTransit.MessageHeaders;

namespace DEAT.WebAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly Dictionary<System.UInt128, Account> _accounts = new();

        public AccountService(ILogger<AccountService> logger)
        {
            _logger = logger;

            //seed accounts
            //Assets
            System.UInt128 id = 100;
            _accounts.Add(id, new Account { AccountId = id, Category = "Assets", AccountName = "Bank", Debit = 1000, Credit = 0 });

            //Asssets - wallets
            id = 101;
            _accounts.Add(id, new Account { AccountId = id, Category = "Assets", AccountName = "Hot wallet - BTC", Debit = 2000, Credit = 200 });
            id = 102;
            _accounts.Add(id, new Account { AccountId = id, Category = "Assets", AccountName = "Hot wallet - SOL", Debit = 1000, Credit = 100 });

            //Liabilities
            id = 200;
            _accounts.Add(id, new Account { AccountId = id, Category = "Liabilities", AccountName = "Accounts Payable", Debit = 2000, Credit = 0 });
            id = 201;
            _accounts.Add(id, new Account { AccountId = id, Category = "Liabilities", AccountName = "Liquidity Provider", Debit = 2000, Credit = 0 });

            //Income
            id = 301;
            _accounts.Add(id, new Account { AccountId = id, Category = "Income", AccountName = "Accounts Recievable", Debit = 0, Credit = 0 });
            id = 302;
            _accounts.Add(id, new Account { AccountId = id, Category = "Income", AccountName = "Withdrawal Fees", Debit = 300, Credit = 0 });
            id = 303;
            _accounts.Add(id, new Account { AccountId = id, Category = "Income", AccountName = "Deposit Fees", Debit = 200, Credit = 0 });
            id = 304;
            _accounts.Add(id, new Account { AccountId = id, Category = "Income", AccountName = "Brokerage Fees", Debit = 0, Credit = 0 });
            id = 305;
            _accounts.Add(id, new Account { AccountId = id, Category = "Income", AccountName = "Commission", Debit = 1000, Credit = 0 });

            //Expenses
            id = 306;
            _accounts.Add(id, new Account { AccountId = id, Category = "Expenses", AccountName = "Network Fees", Debit = 200, Credit = 0 });
            id = 307;
            _accounts.Add(id, new Account { AccountId = id, Category = "Expenses", AccountName = "Merchant Fees", Debit = 100, Credit = 0 });

            //Equity
            id = 400;
            _accounts.Add(id, new Account { AccountId = id, Category = "Equity", AccountName = "Capital", Debit = 70001, Credit = 0 });
        }

        public Task<List<Account>> GetAllAccountsAsync()
        {
            return Task.FromResult(_accounts.Values.ToList());
        }

        public async Task<System.UInt128?> CreateAccountAsync(Account account)
        {
            if (account.AccountId.HasValue && _accounts.ContainsKey(account.AccountId.Value))
            {
                _logger.LogWarning($"Account already exists {account.AccountId}");
                return account.AccountId;
            }

            // Generate a new Account ID
            var newId = _accounts.Keys.Max() + 1;
            account.AccountId = newId;
            account.Debit = account.Debit ?? 0;
            account.Credit = account.Credit ?? 0;

            _accounts.Add(newId, account);

            return newId;
        }

        public Task<bool> DebitAccountAsync(System.UInt128? accountId, System.UInt128? amount)
        {
            if (!accountId.HasValue || !amount.HasValue || !_accounts.ContainsKey(accountId.Value))
                return Task.FromResult(false);

            var account = _accounts[accountId.Value];
            var currentBalance = account.Balance ?? 0;

            if (currentBalance < amount.Value)
                return Task.FromResult(false);

            account.Debit = (account.Debit ?? 0) + amount.Value;
            return Task.FromResult(true);
        }

        public Task<bool> CreditAccountAsync(System.UInt128? accountId, System.UInt128? amount)
        {
            if (!accountId.HasValue || !amount.HasValue || !_accounts.ContainsKey(accountId.Value))
                return Task.FromResult(false);

            var account = _accounts[accountId.Value];
            account.Credit = (account.Credit ?? 0) + amount.Value;
            return Task.FromResult(true);
        }
    }
}
