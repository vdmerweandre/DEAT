using DEAT.AdminUI.Services.Contracts;
using DEAT.Data.Models.Dtos;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace DEAT.AdminUI.Services
{
    public class RoleService : IRoleService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RoleService> _logger;
        private const string _baseUri = "/api/roles";

        public RoleService(
            IHttpClientFactory httpClientFactory,
            ILogger<RoleService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<RoleDto>> GetRolesAsync()
        {
            using var client = _httpClientFactory.CreateClient("WebApi");

            try
            {
                var roles = await client.GetFromJsonAsync<List<RoleDto>>(
                    _baseUri,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                return roles ?? Enumerable.Empty<RoleDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles");
                throw;
            }
        }

        public async Task<RoleDto?> GetRoleAsync(int id)
        {
            using var client = _httpClientFactory.CreateClient("WebApi");

            try
            {
                return await client.GetFromJsonAsync<RoleDto>(
                    $"{_baseUri}/{id}",
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role {RoleId}", id);
                throw;
            }
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleDto role)
        {
            using var client = _httpClientFactory.CreateClient("WebApi");

            try
            {
                var response = await client.PostAsJsonAsync(_baseUri, role);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<RoleDto>() 
                    ?? throw new InvalidOperationException("Failed to deserialize response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                throw;
            }
        }

        public async Task<RoleDto> UpdateRoleAsync(int id, UpdateRoleDto role)
        {
            using var client = _httpClientFactory.CreateClient("WebApi");

            try
            {
                var response = await client.PutAsJsonAsync($"{_baseUri}/{id}", role);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<RoleDto>() 
                    ?? throw new InvalidOperationException("Failed to deserialize response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {RoleId}", id);
                throw;
            }
        }

        public async Task DeleteRoleAsync(int id)
        {
            using var client = _httpClientFactory.CreateClient("WebApi");

            try
            {
                var response = await client.DeleteAsync($"{_baseUri}/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", id);
                throw;
            }
        }
    }
} 