using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Softwares"),PrimaryKey(nameof(Id))]
public class Software
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "nvarchar(150)")]
    public string Name { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "nvarchar(500)")]
    public string Description { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string ActualVersion { get; set; } = string.Empty;
    
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<SoftwareCost> SoftwareCosts { get; set; } = new List<SoftwareCost>();
    public ICollection<SoftDisc> SoftDiscs { get; set; } = new List<SoftDisc>();
    public ICollection<Version> Versions { get; set; } = new List<Version>();
}