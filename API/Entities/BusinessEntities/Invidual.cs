using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Individuals"),PrimaryKey(nameof(Pesel))]
public class Invidual
{
    [Key]
    [Column(TypeName = "nchar(11)")]
    public string Pesel { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "nvarchar(16)")]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public int AddressId { get; set; }
    
    [ForeignKey(nameof(AddressId))]
    public Address Address { get; set; } = null!;

    public virtual Entity Entity { get; set; } = null!;
}