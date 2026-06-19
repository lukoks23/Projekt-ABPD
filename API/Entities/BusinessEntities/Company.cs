using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Companies"),PrimaryKey(nameof(Krs))]
public class Company
{
    [Key]
    [Column(TypeName = "nchar(10)")]
    public string Krs { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public int AddressId { get; set; }
    
    [ForeignKey(nameof(AddressId))]
    public virtual Address Address { get; set; } = null!;

    public virtual Entity Entity { get; set; } = null!;
}