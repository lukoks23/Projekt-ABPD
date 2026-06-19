using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("SoftwareCategories"),PrimaryKey(nameof(Id))]
public class SoftwareCategory
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string Category { get; set; } = string.Empty;
    
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}