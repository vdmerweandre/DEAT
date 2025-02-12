using DEAT.AdminUI.Services.Contracts;
using DEAT.Data.Models.Dtos;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;

namespace DEAT.AdminUI.Services
{
    public class AuditService(
        IHttpClientFactory httpClientFactory,
        ILogger<AuditService> logger) : IAuditService
    {
        private const string _baseUri = "/api/audit";

        public async Task<IEnumerable<EventLog>> GetEventLogsAsync()
        {
            // Create the client
            using HttpClient client = httpClientFactory.CreateClient("WebApi");

            try
            {
                // Make HTTP GET request
                // Parse JSON response deserialize into AccountDto types
                IEnumerable<EventLog>? log = await client.GetFromJsonAsync<IEnumerable<EventLog>>(
                    _baseUri + "/events",
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                return log.OrderByDescending(l => l.Timestamp) ?? Enumerable.Empty<EventLog>();
            }
            catch (Exception ex)
            {
                logger.LogError("Error getting GetAllTransactionsAsync: {Error}", ex);
            }

            return Enumerable.Empty<EventLog>();
        }

        public async Task<IEnumerable<StateChangeLog>> GetStateChangesAsync()
        {
            // Create the client
            using HttpClient client = httpClientFactory.CreateClient("WebApi");

            try
            {
                // Make HTTP GET request
                // Parse JSON response deserialize into AccountDto types
                IEnumerable<StateChangeLog>? log = await client.GetFromJsonAsync<IEnumerable<StateChangeLog>>(
                    _baseUri + "/states",
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                return log.OrderByDescending(l => l.Timestamp) ?? Enumerable.Empty<StateChangeLog>(); 
            }
            catch (Exception ex)
            {
                logger.LogError("Error getting GetAllTransactionsAsync: {Error}", ex);
            }

            return Enumerable.Empty<StateChangeLog>();
        }
    }
}
