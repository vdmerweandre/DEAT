using DEAT.AdminUI.Services.Contracts;
using DEAT.Data.Models.Dtos;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace DEAT.AdminUI.Services
{
    public class AccountService(
        IHttpClientFactory httpClientFactory,
        ILogger<AccountService> logger) : IAccountService
    {
        private const string _baseUri = "/api/accounts";

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return new List<Account>()
            {
                new Account()
                {
                    AccountId = 1001,
                    AccountName = "BTC - Hot wallet",
                    Category = "Asset",
                    Credit = 8000,
                    Debit = 10000
                },
                new Account()
                {
                    AccountId = 1001,
                    AccountName = "BTC - Warm wallet",
                    Category = "Asset",
                    Credit = 1000,
                    Debit = 2000000
                },
                new Account()
                {
                    AccountId = 1001,
                    AccountName = "BTC - Network Fee",
                    Category = "Expenses",
                    Credit = 0,
                    Debit = 2
                }
            };
            
            // Create the client
            using HttpClient client = httpClientFactory.CreateClient("WebApi");

            try
            {
                // Make HTTP GET request
                // Parse JSON response deserialize into AccountDto types
                List<Account>? accounts = await client.GetFromJsonAsync<List<Account>>(
                    _baseUri,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                return accounts ?? Enumerable.Empty<Account>();
            }
            catch (Exception ex)
            {
                logger.LogError("Error getting GetAllTransactionsAsync: {Error}", ex);
            }

            return Enumerable.Empty<Account>();
        }

        public async Task<System.UInt128> CreateAccountAsync(Account account)
        {
            // Create the client
            using HttpClient client = httpClientFactory.CreateClient("WebApi");

            try
            {
                // Make HTTP POST request
                var response = await client.PostAsJsonAsync<Account>(
                    _baseUri,
                    account,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new HttpRequestException("An account with this name already exists", null, System.Net.HttpStatusCode.Conflict);
                }

                response.EnsureSuccessStatusCode();

                var createdAccount = await response.Content.ReadFromJsonAsync<Account>();
                if (createdAccount?.AccountId == null)
                {
                    throw new Exception("Failed to create account: No account ID returned from server");
                }
                return createdAccount.AccountId.Value;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError("Error creating an Account: {Error}", ex);
                throw; // Re-throw the original exception to preserve the status code
            }
            catch (Exception ex)
            {
                logger.LogError("Error creating an Account: {Error}", ex);
                throw new HttpRequestException($"Failed to create account: {ex.Message}", ex, System.Net.HttpStatusCode.Conflict);
            }
        }
    }
}
