using DEAT.Data.Models.Dtos;
using DEAT.WebAPI.TigerBeetleServices.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using TigerBeetle;
using Xunit;
using DtoAccount = DEAT.Data.Models.Dtos.Account;
using TbAccount = TigerBeetle.Account;

namespace DEAT.WebAPI.TigerBeetleServices.Tests;

public class AccountServiceTests
{
    private readonly Mock<ITigerBeetleClient> _mockClient;
    private readonly Mock<ILogger<AccountService>> _mockLogger;
    private readonly Mock<IAccountRegistry> _mockRegistry;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _mockClient = new Mock<ITigerBeetleClient>();
        _mockLogger = new Mock<ILogger<AccountService>>();
        _mockRegistry = new Mock<IAccountRegistry>();
        _accountService = new AccountService(_mockClient.Object, _mockLogger.Object, _mockRegistry.Object);
    }

    [Fact]
    public async Task CreateAccountAsync_WithoutAccountId_ShouldCreateAccount()
    {
        // Arrange
        var accountDto = new DtoAccount
        {
            AccountName = "Test Account",
            Category = "Assets"
        };

        _mockClient.Setup(c => c.CreateAccountsAsync(It.IsAny<TbAccount[]>()))
            .ReturnsAsync(Array.Empty<CreateAccountsResult>());

        // Act
        var result = await _accountService.CreateAccountAsync(accountDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accountDto.AccountName, result.AccountName);
        Assert.Equal(accountDto.Category, result.Category);
        Assert.True(result.AccountId.HasValue);

        _mockClient.Verify(c => c.CreateAccountsAsync(
            It.Is<TbAccount[]>(accounts =>
                accounts.Length == 1 &&
                accounts[0].Code == 1)), // Assets category code
            Times.Once);

        _mockRegistry.Verify(r => r.AddAccount(
            It.IsAny<UInt128>(),
            It.Is<string>(name => name == accountDto.AccountName)),
            Times.Once);
    }

    [Fact]
    public async Task CreateAccountAsync_WhenAccountExists_ShouldThrowException()
    {
        // Arrange
        var existingId = new UInt128(1, 0);
        var accountDto = new DtoAccount
        {
            AccountId = existingId,
            AccountName = "Test Account",
            Category = "Assets"
        };

        _mockClient.Setup(c => c.CreateAccountsAsync(It.Is<TbAccount[]>(
            accounts => accounts.Length == 1 && accounts[0].Id == existingId)))
            .ReturnsAsync(new[] { new CreateAccountsResult { Result = CreateAccountResult.Exists } });

        // Act & Assert
        await Assert.ThrowsAsync<AccountAlreadyExistsException>(
            () => _accountService.CreateAccountAsync(accountDto)
        );
    }

    [Fact]
    public async Task GetAllAccountsAsync_ShouldReturnAllAccounts()
    {
        // Arrange
        var accountId = new UInt128(1, 0);
        var tbAccount = new TbAccount
        {
            Id = accountId,
            Code = 1 // Assets
        };

        _mockRegistry.Setup(r => r.GetAllAccountIds())
            .Returns(new[] { accountId });

        _mockRegistry.Setup(r => r.GetAccountName(accountId))
            .Returns("Test Account");

        _mockClient.Setup(c => c.LookupAccountsAsync(It.Is<UInt128[]>(ids =>
            ids.Contains(accountId))))
            .ReturnsAsync(new[] { tbAccount });

        // Act
        var result = await _accountService.GetAllAccountsAsync();

        // Assert
        var resultList = result.ToList();
        Assert.Single(resultList);
        Assert.Equal("Assets", resultList[0].Category);
        Assert.Equal("Test Account", resultList[0].AccountName);
    }

    [Fact]
    public async Task GetAccountAsync_WhenAccountExists_ShouldReturnAccount()
    {
        // Arrange
        var accountId = new UInt128(1, 0);
        var tbAccount = new TbAccount
        {
            Id = accountId,
            Code = 1 // Assets
        };

        _mockRegistry.Setup(r => r.GetAccountName(accountId))
            .Returns("Test Account");

        _mockClient.Setup(c => c.LookupAccountsAsync(It.Is<UInt128[]>(
            ids => ids.Length == 1 && ids[0] == accountId)))
            .ReturnsAsync(new[] { tbAccount });

        // Act
        var result = await _accountService.GetAccountAsync(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accountId, result.AccountId);
        Assert.Equal("Assets", result.Category);
        Assert.Equal("Test Account", result.AccountName);
    }

    [Fact]
    public async Task GetAccountAsync_WhenAccountDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var accountId = new UInt128(1, 0);

        _mockClient.Setup(c => c.LookupAccountsAsync(It.Is<UInt128[]>(
            ids => ids.Length == 1 && ids[0] == accountId)))
            .ReturnsAsync(Array.Empty<TbAccount>());

        // Act
        var result = await _accountService.GetAccountAsync(accountId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DebitAccountAsync_ShouldCreateTransfer()
    {
        // Arrange
        var accountId = new UInt128(1, 0);
        var amount = new UInt128(100, 0);

        _mockClient.Setup(c => c.CreateTransfersAsync(It.IsAny<Transfer[]>()))
            .ReturnsAsync(Array.Empty<CreateTransfersResult>());

        // Act
        var result = await _accountService.DebitAccountAsync(accountId, amount);

        // Assert
        Assert.True(result);
        _mockClient.Verify(c => c.CreateTransfersAsync(
            It.Is<Transfer[]>(transfers =>
                transfers.Length == 1 &&
                transfers[0].DebitAccountId == accountId &&
                transfers[0].Amount == amount)),
            Times.Once);
    }

    [Fact]
    public async Task CreditAccountAsync_ShouldCreateTransfer()
    {
        // Arrange
        var accountId = new UInt128(1, 0);
        var amount = new UInt128(100, 0);

        _mockClient.Setup(c => c.CreateTransfersAsync(It.IsAny<Transfer[]>()))
            .ReturnsAsync(Array.Empty<CreateTransfersResult>());

        // Act
        var result = await _accountService.CreditAccountAsync(accountId, amount);

        // Assert
        Assert.True(result);
        _mockClient.Verify(c => c.CreateTransfersAsync(
            It.Is<Transfer[]>(transfers =>
                transfers.Length == 1 &&
                transfers[0].CreditAccountId == accountId &&
                transfers[0].Amount == amount)),
            Times.Once);
    }
}