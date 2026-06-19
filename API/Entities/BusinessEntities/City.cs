using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Cities"),PrimaryKey(nameof(Id))]
public class City
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string Name { get; set; } = string.Empty;
    [Required]
    public int CountryId { get; set; }

    [ForeignKey(nameof(CountryId))]
    public virtual Country Country { get; set; } = null!;
    
    public virtual ICollection<Street> Streets { get; set; } = new List<Street>();
}