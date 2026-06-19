using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities;

[Table("Roles"),PrimaryKey(nameof(Id))]
public class Role
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string Name { get; set; } = string.Empty;
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}