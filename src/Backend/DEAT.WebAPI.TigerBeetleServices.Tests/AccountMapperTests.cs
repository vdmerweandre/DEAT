using TigerBeetle;
using DtoAccount = DEAT.Data.Models.Dtos.Account;
using TbAccount = TigerBeetle.Account;

namespace DEAT.WebAPI.TigerBeetleServices.Tests;

public class AccountMapperTests
{
    [Fact]
    public void GetAllCategories_ShouldReturnAllCategories()
    {
        // Arrange & Act
        var categories = AccountMapper.GetAllCategories().ToList();

        // Assert
        Assert.Equal(5, categories.Count);
        Assert.Contains("Assets", categories);
        Assert.Contains("Liabilities", categories);
        Assert.Contains("Equity", categories);
        Assert.Contains("Income", categories);
        Assert.Contains("Expenses", categories);
    }

    [Fact]
    public void GetCategoryCode_ShouldReturnCorrectCode()
    {
        // Arrange & Act & Assert
        Assert.Equal(1, AccountMapper.GetCategoryCode("Assets"));
        Assert.Equal(2, AccountMapper.GetCategoryCode("Liabilities"));
        Assert.Equal(3, AccountMapper.GetCategoryCode("Equity"));
        Assert.Equal(4, AccountMapper.GetCategoryCode("Income"));
        Assert.Equal(5, AccountMapper.GetCategoryCode("Expenses"));
        Assert.Equal(0, AccountMapper.GetCategoryCode("Invalid"));
    }

    [Fact]
    public void ToTigerBeetleAccount_WithExistingId_ShouldUseProvidedId()
    {
        // Arrange
        var accountId = new UInt128(1, 0);
        var account = new DtoAccount
        {
            AccountId = accountId,
            Category = "Assets",
            AccountName = "Test Account"
        };

        // Act
        var result = account.ToTigerBeetleAccount();

        // Assert
        Assert.Equal(accountId, result.Id);
        Assert.Equal(1, result.Code); // Assets category code
        Assert.Equal(AccountFlags.DebitsMustNotExceedCredits, result.Flags);
    }

    [Fact]
    public void ToTigerBeetleAccount_WithoutId_ShouldGenerateDeterministicId()
    {
        // Arrange
        var account = new DtoAccount
        {
            Category = "Assets",
            AccountName = "Test Account"
        };

        // Act
        var result = account.ToTigerBeetleAccount();

        // Assert
        Assert.NotEqual(UInt128.Zero, result.Id);
        Assert.Equal(1, result.Code); // Assets category code
        Assert.Equal(AccountFlags.DebitsMustNotExceedCredits, result.Flags);

        // Verify deterministic ID generation
        var result2 = account.ToTigerBeetleAccount();
        Assert.Equal(result.Id, result2.Id);
    }

    [Fact]
    public void ToDtoAccount_ShouldMapCorrectly()
    {
        // Arrange
        var accountId = new UInt128(1, 0);
        var accountName = "Test Account";

        // Create a TigerBeetle account
        var tbAccount = new TbAccount
        {
            Id = accountId,
            Code = 1 // Assets
        };

        // Act
        var result = tbAccount.ToDtoAccount(accountName);

        // Assert
        Assert.Equal(accountId, result.AccountId);
        Assert.Equal("Assets", result.Category);
        Assert.Equal(accountName, result.AccountName);

        // We can't test Debit and Credit values since they're read-only
        // and we can't set them in the test
    }

    [Fact]
    public void ToDtoAccount_WithoutName_ShouldGenerateDefaultName()
    {
        // Arrange
        var accountId = new UInt128(1, 0);
        var tbAccount = new TbAccount
        {
            Id = accountId,
            Code = 1 // Assets
        };

        // Act
        var result = tbAccount.ToDtoAccount();

        // Assert
        Assert.Equal(accountId, result.AccountId);
        Assert.Equal("Assets", result.Category);
        Assert.Equal($"Assets_{accountId}", result.AccountName);
    }

    [Theory]
    [InlineData("Assets", AccountFlags.DebitsMustNotExceedCredits)]
    [InlineData("Expenses", AccountFlags.DebitsMustNotExceedCredits)]
    [InlineData("Liabilities", AccountFlags.CreditsMustNotExceedDebits)]
    [InlineData("Equity", AccountFlags.CreditsMustNotExceedDebits)]
    [InlineData("Income", AccountFlags.CreditsMustNotExceedDebits)]
    [InlineData("Invalid", AccountFlags.None)]
    public void ToTigerBeetleAccount_ShouldSetCorrectFlags(string category, AccountFlags expectedFlags)
    {
        // Arrange
        var account = new DtoAccount
        {
            Category = category,
            AccountName = "Test Account"
        };

        // Act
        var tbAccount = AccountMapper.ToTigerBeetleAccount(account);

        // Assert
        Assert.Equal(expectedFlags, tbAccount.Flags);
    }
}