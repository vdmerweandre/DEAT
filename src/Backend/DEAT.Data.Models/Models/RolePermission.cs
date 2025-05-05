using System.ComponentModel.DataAnnotations;

namespace DEAT.Data.Models.Models
{
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Resource { get; set; } = string.Empty;

        [Required]
        public int PermissionValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
} 