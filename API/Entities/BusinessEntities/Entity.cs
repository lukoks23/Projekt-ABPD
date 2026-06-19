using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Entities"),PrimaryKey(nameof(Id))]
public class Entity
{
    [Key]
    public int Id { get; set; }
    [Column(TypeName = "nchar(11)")]
    public string? Pesel { get; set; }
    [Column(TypeName = "nchar(10)")]
    public string? Krs { get; set; }

    [ForeignKey(nameof(Pesel))]
    public Invidual? Individual { get; set; }
    [ForeignKey(nameof(Krs))]
    public Company? Company { get; set; }
    
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}