using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using static MassTransit.MessageHeaders;

namespace DEAT.WebAPI.Services
{
    public class AccountService: IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly Dictionary<Guid, Account> _accounts = new();

        public AccountService(ILogger<AccountService> logger)
        { 
            _logger = logger;

            //Assets (A) + Expenses (E) = Liabilities (L) + Equity/Capital (C) + Income (I).
            //Assets: Items that a business owns that add value to the company
            //Liabilities: What a business owes to other parties
            //Income: Money that a business earns
            //Expenses: Costs that a business incurs to generate revenue

            //Withdrawal: Wallets (A) (debit) + Network Fees (E) (credit) = Account Payable (L) (credit) + Withdrawal Fees (I) (credit) + Buy-backs (L) (debit)
            // -100 + 5 = 90 + 10 - 5
            //settlement/buying back fees
            // - = Bank (C) (debit) + Buy-backs (L) (credit)
            // - = -5 + 5

            //Deposit
            //Wallets(A)(credit) = Account Payable(L)(debit) + Deposit Fees(I)(credit)
            // 100 = -110 + 10

            //OTC: - = Account Payable (L) (debit) + Account Recievable (I) (credit) + Commission(I) (credit)
            // - = -100 + 90 + 10

            //seed accounts
            //Assets
            Guid id = Guid.Parse("c4371215-948f-4d2f-8097-9312c53f9f21");
            _accounts.Add(id, new Account { AccountId = id, Category = "Assets", AccountName = "Bank", Debit = 1000, Credit = 0 });

            //Asssets - wallets
            id = Guid.Parse("31beb218-f9be-44e0-bbd3-29448fe60d9a");
            _accounts.Add(id, new Account { AccountId = id, Category = "Assets", AccountName = "Hot wallet - BTC", Debit = 2000, Credit = 200 });
            id = Guid.Parse("1db0182b-5a4e-42dc-b4c5-ddeb6e5ed2c4");
            _accounts.Add(id, new Account { AccountId = id, Category = "Assets", AccountName = "Hot wallet - SOL", Debit = 1000, Credit = 100 });

            //Liabilities
            id = Guid.Parse("7ccdffbd-c698-40e8-ba8b-09010beb0e85");
            _accounts.Add(id, new Account { AccountId = id, Category = "Liabilities", AccountName = "Accounts Payable", Debit = 2000, Credit = 0 });

            //Income
            id = Guid.Parse("e5b56834-f9dc-4d88-b909-35bb94394cc5");
            _accounts.Add(id, new Account { AccountId = id, Category = "Income", AccountName = "Accounts Recievable", Debit = 0, Credit = 0 });
            id = Guid.Parse("39941382-149e-4964-b725-ad66aa4325ad");
            _accounts.Add(id, new Account { AccountId = id, Category = "Income", AccountName = "Withdrawal Fees", Debit = 300, Credit = 0 });
            id = Guid.Parse("c2953603-2622-433e-a96c-39aba8fab744");
            _accounts.Add(id, new Account { AccountId = id, Category = "Income", AccountName = "Deposit Fees", Debit = 200, Credit = 0 });
            id = Guid.Parse("0c7ffe5d-9eba-40c2-b649-3f0a7b6cdbac");
            _accounts.Add(id, new Account { AccountId = id, Category = "Income", AccountName = "Commission", Debit = 1000, Credit = 0 });

            //Expenses
            id = Guid.Parse("c4178afd-e32c-4fd9-8c4d-25e78c522a5c");
            _accounts.Add(id, new Account { AccountId = id, Category = "Expenses", AccountName = "Network Fees", Debit = 200, Credit = 0 });
            id = Guid.Parse("e10278dc-c66a-4c97-868b-2de6bf0bede2");
            _accounts.Add(id, new Account { AccountId = id, Category = "Expenses", AccountName = "Merchant Fees", Debit = 100, Credit = 0 });

            //Equity
            id = Guid.Parse("b772c40b-c306-421c-a2e7-41187259f905");
            _accounts.Add(id, new Account { AccountId = id, Category = "Equity", AccountName = "Capital", Debit = 70001, Credit = 0 });
        }

        public Task<List<Account>> GetAllAccountsAsync()
        {
            return Task.FromResult(_accounts.Values.ToList());
        }

        public async Task<Guid> CreateAccountAsync(Account account)
        {
            if (_accounts.ContainsKey(account.AccountId))
            {
                _logger.LogWarning($"Account already exists {account.AccountId}");
                return await Task.FromResult(account.AccountId);
            }

            // Generate a new Account ID
            account.AccountId = Guid.NewGuid();

            _accounts.Add(account.AccountId, account);

            return await Task.FromResult(account.AccountId);
        }

        public Task<bool> DebitAccountAsync(Guid accountId, decimal amount)
        {
            if (!_accounts.ContainsKey(accountId) || _accounts[accountId].Balance < amount)
                return Task.FromResult(false);

            _accounts[accountId].Debit += amount;
            return Task.FromResult(true);
        }

        public Task<bool> CreditAccountAsync(Guid accountId, decimal amount)
        {
            if (!_accounts.ContainsKey(accountId))
                return Task.FromResult(false);

            _accounts[accountId].Credit += amount;
            return Task.FromResult(true);
        }
    }
}
