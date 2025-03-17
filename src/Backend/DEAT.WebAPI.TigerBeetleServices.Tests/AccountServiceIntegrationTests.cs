using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Account = DEAT.Data.Models.Dtos.Account;

namespace DEAT.WebAPI.TigerBeetleServices.Tests;

[Collection("Integration Tests")]
public class AccountServiceIntegrationTests : IDisposable
{
    private readonly IAccountService _accountService;
    private readonly IAccountRegistry _accountRegistry;
    private readonly ServiceProvider _serviceProvider;

    public AccountServiceIntegrationTests()
    {
        // Setup dependency injection
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging(builder => builder.AddConsole());

        // Add TigerBeetle services
        services.AddTigerBeetleServices(new[] { "127.0.0.1:3000" }, 0);

        _serviceProvider = services.BuildServiceProvider();

        // Get services from DI
        _accountService = _serviceProvider.GetRequiredService<IAccountService>();
        _accountRegistry = _serviceProvider.GetRequiredService<IAccountRegistry>();
    }

    [Fact]
    public async Task CreateAccountAsync_WithoutAccountId_ShouldCreateNewAccount()
    {
        // Arrange
        var uniqueName = $"Integration Test Account {Guid.NewGuid()}";
        var accountDto = new Account
        {
            Category = "Assets",
            AccountName = uniqueName
        };

        // Act
        var result = await _accountService.CreateAccountAsync(accountDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AccountId);
        Assert.NotEqual(UInt128.Zero, result.AccountId);
        Assert.Equal("Assets", result.Category);
        Assert.Equal(uniqueName, result.AccountName);

        // Verify the account is in the registry
        Assert.True(_accountRegistry.Contains(result.AccountId.Value));
        Assert.Equal(uniqueName, _accountRegistry.GetAccountName(result.AccountId.Value));

        // Verify we can retrieve the created account
        var createdAccount = await _accountService.GetAccountAsync(result.AccountId);
        Assert.NotNull(createdAccount);
        Assert.Equal(result.AccountId, createdAccount.AccountId);
        Assert.Equal(result.Category, createdAccount.Category);
        Assert.Equal(result.AccountName, createdAccount.AccountName);

        // Verify we can get all accounts and find our new account
        var allAccounts = await _accountService.GetAllAccountsAsync();
        var foundAccount = allAccounts.FirstOrDefault(a => a.AccountId == result.AccountId);
        Assert.NotNull(foundAccount);
        Assert.Equal(result.AccountName, foundAccount.AccountName);
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
} 