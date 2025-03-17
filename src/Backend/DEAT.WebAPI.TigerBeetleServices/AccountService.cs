using System.Collections.Concurrent;
using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.TigerBeetleServices.Exceptions;
using Microsoft.Extensions.Logging;
using TigerBeetle;

namespace DEAT.WebAPI.TigerBeetleServices;

#nullable enable

public class AccountService : IAccountService
{
    private readonly ITigerBeetleClient _client;
    private readonly ILogger<AccountService> _logger;
    private readonly IAccountRegistry _accountRegistry;

    public AccountService(ITigerBeetleClient client, ILogger<AccountService> logger, IAccountRegistry accountRegistry)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _accountRegistry = accountRegistry ?? throw new ArgumentNullException(nameof(accountRegistry));
        
        // Seed accounts on startup
        SeedAccountsAsync().GetAwaiter().GetResult();
    }

    private async Task SeedAccountsAsync()
    {
        try
        {
            _logger.LogInformation("Seeding accounts from AccountDefinitions");
            
            // Create TigerBeetle accounts from definitions
            var accounts = AccountDefinitions.Accounts.Select(a => 
                AccountDefinitions.CreateAccount(a.Id, a.Name, a.Flags, a.Code)).ToArray();
            
            // Try to create accounts in TigerBeetle
            var results = await _client.CreateAccountsAsync(accounts);
            
            // Store account names in the registry
            foreach (var account in AccountDefinitions.Accounts)
            {
                _accountRegistry.AddAccount(account.Id, account.Name);
                _logger.LogInformation("Added account to registry: {Id} -> {Name}", account.Id, account.Name);
            }
            
            _logger.LogInformation("Account seeding completed. {SuccessCount} accounts created, {ExistingCount} already existed", 
                results?.Count(r => r.Result == CreateAccountResult.Ok) ?? accounts.Length,
                results?.Count(r => r.Result == CreateAccountResult.Exists) ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding accounts");
        }
    }

    public void AddAccountName(UInt128 accountId, string name)
    {
        _accountRegistry.SetAccountName(accountId, name);
    }

    public async Task<Data.Models.Dtos.Account> CreateAccountAsync(Data.Models.Dtos.Account account)
    {
        try
        {
            _logger.LogInformation("Creating account with name: {AccountName}, category: {Category}", 
                account.AccountName, account.Category);
            
            var tbAccount = account.ToTigerBeetleAccount();
            _logger.LogInformation("Generated TigerBeetle account with ID: {AccountId}", tbAccount.Id);
            
            var result = await _client.CreateAccountsAsync(new[] { tbAccount });
            _logger.LogInformation("CreateAccountsAsync result: {Result}", 
                result == null ? "null" : $"Length: {result.Length}");

            // No result means success according to TigerBeetle docs
            if (result == null || result.Length == 0)
            {
                account.AccountId = tbAccount.Id;
                _accountRegistry.AddAccount(tbAccount.Id, account.AccountName);
                _logger.LogInformation("Successfully created account with ID: {AccountId}", account.AccountId);
                return account;
            }

            if (result[0].Result == CreateAccountResult.Exists)
            {
                throw new AccountAlreadyExistsException($"Account with ID {tbAccount.Id} already exists");
            }

            throw new Exception($"Failed to create account: {result[0].Result}");
        }
        catch (AccountAlreadyExistsException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            throw;
        }
    }

    public async Task<IEnumerable<Data.Models.Dtos.Account>> GetAllAccountsAsync()
    {
        try
        {
            var accounts = new List<Data.Models.Dtos.Account>();
            foreach (var category in AccountMapper.GetAllCategories())
            {
                var categoryAccounts = await GetAccountsByCategory(category);
                accounts.AddRange(categoryAccounts);
            }

            return accounts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all accounts");
            throw;
        }
    }

    public async Task<Data.Models.Dtos.Account?> GetAccountAsync(UInt128? accountId)
    {
        ArgumentNullException.ThrowIfNull(accountId);

        try
        {
            _logger.LogInformation("Looking up account with ID: {AccountId}", accountId.Value);
            var accounts = await _client.LookupAccountsAsync(new[] { accountId.Value });
            _logger.LogInformation("LookupAccountsAsync result: {Result}", 
                accounts == null ? "null" : $"Length: {accounts.Length}");

            if (accounts != null && accounts.Length > 0)
            {
                var tbAccount = accounts[0];
                _logger.LogInformation("Found TigerBeetle account: ID={Id}, Code={Code}, Flags={Flags}", 
                    tbAccount.Id, tbAccount.Code, tbAccount.Flags);

                var accountName = _accountRegistry.GetAccountName(accountId.Value);
                _logger.LogInformation("Found account name in registry: {AccountName}", accountName);

                var dtoAccount = tbAccount.ToDtoAccount(accountName);
                _logger.LogInformation("Mapped to DTO account: ID={Id}, Category={Category}, Name={Name}", 
                    dtoAccount.AccountId, dtoAccount.Category, dtoAccount.AccountName);

                return dtoAccount;
            }

            _logger.LogWarning("No account found with ID: {AccountId}", accountId.Value);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account {AccountId}", accountId);
            throw;
        }
    }

    public async Task<bool> DebitAccountAsync(UInt128? accountId, UInt128? amount)
    {
        if (!accountId.HasValue || !amount.HasValue)
        {
            return false;
        }

        try
        {
            var transfer = new Transfer
            {
                Id = GenerateTransferId(),
                DebitAccountId = accountId.Value,
                CreditAccountId = accountId.Value,
                Amount = amount.Value,
                Ledger = 1,
                Code = 0,
                Flags = TransferFlags.None
            };

            var result = await _client.CreateTransfersAsync(new[] { transfer });
            return result.Length == 0 || result[0].Result == CreateTransferResult.Ok;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error debiting account {AccountId}", accountId);
            return false;
        }
    }

    public async Task<bool> CreditAccountAsync(UInt128? accountId, UInt128? amount)
    {
        if (!accountId.HasValue || !amount.HasValue)
        {
            return false;
        }

        try
        {
            var transfer = new Transfer
            {
                Id = GenerateTransferId(),
                DebitAccountId = accountId.Value,
                CreditAccountId = accountId.Value,
                Amount = amount.Value,
                Ledger = 1,
                Code = 0,
                Flags = TransferFlags.None
            };

            var result = await _client.CreateTransfersAsync(new[] { transfer });
            return result.Length == 0 || result[0].Result == CreateTransferResult.Ok;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crediting account {AccountId}", accountId);
            return false;
        }
    }

    private async Task<IEnumerable<Data.Models.Dtos.Account>> GetAccountsByCategory(string category)
    {
        try
        {
            _logger.LogInformation("Getting accounts for category: {Category}", category);
            
            var code = AccountMapper.GetCategoryCode(category);
            _logger.LogInformation("Category code: {Code}", code);
            if (code == 0) return Enumerable.Empty<Data.Models.Dtos.Account>();

            var accountIds = _accountRegistry.GetAllAccountIds().ToArray();
            if (accountIds.Length == 0)
            {
                _logger.LogInformation("No accounts found in the registry");
                return Enumerable.Empty<Data.Models.Dtos.Account>();
            }

            _logger.LogInformation("Looking up {Count} accounts", accountIds.Length);
            var accounts = await _client.LookupAccountsAsync(accountIds);
            _logger.LogInformation("Found {Count} accounts", accounts?.Length ?? 0);

            if (accounts != null && accounts.Length > 0)
            {
                var dtoAccounts = accounts
                    .Where(a => a.Code == code)
                    .Select(a =>
                    {
                        var accountName = _accountRegistry.GetAccountName(a.Id);
                        _logger.LogInformation("Found account: ID={Id}, Code={Code}, Name={Name}", 
                            a.Id, a.Code, accountName);
                        return a.ToDtoAccount(accountName);
                    })
                    .ToList();

                return dtoAccounts;
            }

            return Enumerable.Empty<Data.Models.Dtos.Account>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts for category {Category}", category);
            return Enumerable.Empty<Data.Models.Dtos.Account>();
        }
    }

    private static UInt128 GenerateTransferId()
    {
        var guid = Guid.NewGuid();
        var bytes = new byte[16];
        Array.Copy(guid.ToByteArray(), bytes, 16);
        return new UInt128(BitConverter.ToUInt64(bytes, 8), BitConverter.ToUInt64(bytes, 0));
    }
} 