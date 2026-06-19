using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Countries"),PrimaryKey(nameof(Id))]
public class Country
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string Name { get; set; } = string.Empty;
    
    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}