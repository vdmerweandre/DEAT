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

        public async Task<Guid> CreateAccountAsync(Account account)
        {
            // Create the client
            using HttpClient client = httpClientFactory.CreateClient("WebApi");

            try
            {
                // Make HTTP GET request
                // Parse JSON response deserialize into AccountDto types
                var response = await client.PostAsJsonAsync<Account>(
                    _baseUri,
                    account,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Guid>();
            }
            catch (Exception ex)
            {
                logger.LogError("Error creating and Account: {Error}", ex);
            }

            return Guid.Empty;
        }
    }
}
