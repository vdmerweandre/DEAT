using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace DEAT.WebAPI.Services
{
    public class AccountService: IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly Dictionary<Guid, AccountDto> _accounts = new();

        public AccountService(ILogger<AccountService> logger)
        { 
            _logger = logger;

            //seed accounts
            //Assets
            Guid id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Assets", AccountName = "Regular Checking Account", Debit = 10, Credit = 10001});
            id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Assets", AccountName = "Savings Account", Debit = 0, Credit = 10000 });
            id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Assets", AccountName = "Account Recievable", Debit = 0, Credit = 5000 });
            id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Assets", AccountName = "Prepaid Expenses", Debit = 0, Credit = 2000 });
            id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Assets", AccountName = "Withdrawal Fees", Debit = 0, Credit = 1000 });

            //wallets
            id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Assets", AccountName = "Hot wallet - BTC", Debit = 20, Credit = 20001 });
            id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Assets", AccountName = "Hot wallet - SOL", Debit = 10, Credit = 20002 });

            //Liabilities
            id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Liabilities", AccountName = "Account Payable", Debit = 100, Credit = 50 });
            id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Liabilities", AccountName = "Fees Incurred", Debit = 500, Credit = 50 });

            //Equity
            id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Equity", AccountName = "Retained earnings", Debit = 50001, Credit = 80 });
            id = Guid.NewGuid();
            _accounts.Add(id, new AccountDto { AccountId = id, Category = "Equity", AccountName = "Capital", Debit = 70001, Credit = 1000 });
        }

        public Task<List<AccountDto>> GetAllAccountsAsync()
        {
            return Task.FromResult(_accounts.Values.ToList());
        }

        public async Task<Guid> CreateAccountAsync(AccountDto account)
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
