using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("SoftwareCosts"),PrimaryKey(nameof(Id))]
public class SoftwareCost
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int SoftwareId { get; set; }
    [Required]
    [Column(TypeName = "decimal(15, 2)")]
    public decimal Price { get; set; }
    [Required]
    public int BillingTypeId { get; set; }

    [ForeignKey(nameof(BillingTypeId))]
    public virtual BillingType BillingType { get; set; } = null!;
    [ForeignKey(nameof(SoftwareId))]
    public virtual Software Softwares { get; set; } = null!;
}