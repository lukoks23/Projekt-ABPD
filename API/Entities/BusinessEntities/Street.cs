using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Streets"),PrimaryKey(nameof(Id))]
public class Street
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string Name { get; set; } = string.Empty;
    [Required]
    public int CityId { get; set; }
    
    [ForeignKey(nameof(CityId))]
    public virtual City City { get; set; } = null!;
    
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
}