using DEAT.AdminUI.Services.Contracts;
using DEAT.Data.Models.Dtos;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace DEAT.AdminUI.Services
{
    public class TransactionService(
        IHttpClientFactory httpClientFactory,
        ILogger<TransactionService> logger) : ITransactionService
    {
        private const string _baseUri = "/api/transactions";

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
        {
            // Create the client
            using HttpClient client = httpClientFactory.CreateClient("WebApi");

            try
            {
                // Make HTTP GET request
                // Parse JSON response deserialize into TransactionDto types
                List<TransactionDto>? transactions = await client.GetFromJsonAsync<List<TransactionDto>>(
                    _baseUri,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                return transactions ?? Enumerable.Empty<TransactionDto>();
            }
            catch (Exception ex)
            {
                logger.LogError("Error getting GetAllTransactionsAsync: {Error}", ex);
            }

            return Enumerable.Empty<TransactionDto>();
        }

        public async Task<Guid> CreateTransactionAsync(TransactionDto transction)
        {
            // Create the client
            using HttpClient client = httpClientFactory.CreateClient("WebApi");

            try
            {
                // Make HTTP GET request
                // Parse JSON response deserialize into AccountDto types
                var response = await client.PostAsJsonAsync<TransactionDto>(
                    _baseUri,
                    transction,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                response.EnsureSuccessStatusCode();

                var id =  await response.Content.ReadFromJsonAsync<Guid>();

                return id;
            }
            catch (Exception ex)
            {
                logger.LogError("Error creating a transaction: {Error}", ex);
            }

            return Guid.Empty;
        }

        private async Task<Guid> UpdateTransaction(string url)
        {
            // Create the client
            using HttpClient client = httpClientFactory.CreateClient("WebApi");

            try
            {
                // Make HTTP GET request
                // Parse JSON response deserialize into AccountDto types
                var response = await client.PutAsJsonAsync(
                    url,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                response.EnsureSuccessStatusCode();

                var id = await response.Content.ReadFromJsonAsync<Guid>();

                return id;
            }
            catch (Exception ex)
            {
                logger.LogError("Error updating transaction: {Error}", ex);
            }

            return Guid.Empty;
        }

        public async Task<Guid> ApproveTransactionAsync(Guid transactionId)
        {
            return await UpdateTransaction(_baseUri + $"/{transactionId}/approve");
        }

        public async Task<Guid> CancelTransactionAsync(Guid transactionId)
        {
            return await UpdateTransaction(_baseUri + $"/{transactionId}/cancel");
        }

        public async Task<Guid> RetryTransactionAsync(Guid transactionId)
        {
            return await UpdateTransaction(_baseUri + $"/{transactionId}/retry");
        }

        public async Task<Guid> ConfirmTransactionLegAsync(Guid transactionId, Guid transactionLegId)
        {
            return await UpdateTransaction(_baseUri + $"/{transactionId}/legs/{transactionLegId}/confirm");
        }
    }
}
