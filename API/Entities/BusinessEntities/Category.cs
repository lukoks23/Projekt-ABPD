using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Categories"),PrimaryKey(nameof(Id))]
public class Category
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int CategoryId { get; set; }
    [Required]
    public int SoftwareId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public virtual SoftwareCategory SoftwareCategory { get; set; } = null!;

    [ForeignKey(nameof(SoftwareId))]
    public virtual Software Software { get; set; } = null!;
}