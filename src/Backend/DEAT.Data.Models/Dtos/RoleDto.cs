namespace DEAT.Data.Models.Dtos
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<RolePermissionDto> Permissions { get; set; } = new();
    }

    public class RolePermissionDto
    {
        public int Id { get; set; }
        public string Resource { get; set; } = string.Empty;
        public int PermissionValue { get; set; }
    }

    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<CreateRolePermissionDto> Permissions { get; set; } = new();
    }

    public class CreateRolePermissionDto
    {
        public string Resource { get; set; } = string.Empty;
        public int PermissionValue { get; set; }
    }

    public class UpdateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<UpdateRolePermissionDto> Permissions { get; set; } = new();
    }

    public class UpdateRolePermissionDto
    {
        public int Id { get; set; }
        public string Resource { get; set; } = string.Empty;
        public int PermissionValue { get; set; }
    }
} 