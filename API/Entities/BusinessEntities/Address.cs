using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Addresses"),PrimaryKey(nameof(Id))]
public class Address
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int StreetId { get; set; }
    [Required]
    public int BuildingNumber { get; set; }
    [Required]
    public int? ApartmentNumber { get; set; }
    [Required]
    [Column(TypeName = "varchar(15)")]
    public string PostCode { get; set; } = string.Empty;

    [ForeignKey(nameof(StreetId))]
    public virtual Street Street { get; set; } = null!;

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();
    public virtual ICollection<Invidual> Individuals { get; set; } = new List<Invidual>();
}