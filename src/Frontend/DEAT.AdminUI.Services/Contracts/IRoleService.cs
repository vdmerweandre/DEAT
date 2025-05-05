using DEAT.Data.Models.Dtos;

namespace DEAT.AdminUI.Services.Contracts
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetRolesAsync();
        Task<RoleDto?> GetRoleAsync(int id);
        Task<RoleDto> CreateRoleAsync(CreateRoleDto role);
        Task<RoleDto> UpdateRoleAsync(int id, UpdateRoleDto role);
        Task DeleteRoleAsync(int id);
    }
} 