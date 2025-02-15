using DEAT.AdminUI.Services.Contracts;
using DEAT.Data.Models.Dtos;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace DEAT.AdminUI.Services
{
    public class LedgerService(
        IHttpClientFactory httpClientFactory,
        ILogger<LedgerService> logger) : ILedgerService
    {
        private const string _baseUri = "/api/ledger";

        public async Task<IEnumerable<LedgerEntry>> GetLedgerEntriesAsync()
        {
            // Create the client
            using HttpClient client = httpClientFactory.CreateClient("WebApi");

            try
            {
                // Make HTTP GET request
                // Parse JSON response deserialize into TransactionDto types
                List<LedgerEntry>? ledgerEntries = await client.GetFromJsonAsync<List<LedgerEntry>>(
                    _baseUri,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                return ledgerEntries ?? Enumerable.Empty<LedgerEntry>();
            }
            catch (Exception ex)
            {
                logger.LogError("Error getting GetLedgerEntriesAsync: {Error}", ex);
            }

            return Enumerable.Empty<LedgerEntry>();
        }
    }
}
