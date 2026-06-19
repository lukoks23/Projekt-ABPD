using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities;

[Table("Users"),PrimaryKey(nameof(UserName))]
public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string UserName { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "nvarchar(256)")]
    public string PasswordHash { get; set; } = string.Empty;
    [Column(TypeName = "nvarchar(128)")]
    public string? RefreshToken { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }
    [Required]
    public int RoleId { get; set; }
    
    [ForeignKey(nameof(RoleId))]
    public virtual Role Role { get; set; } = null!;
}