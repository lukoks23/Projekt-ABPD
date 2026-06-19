using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Contracts"),PrimaryKey(nameof(Id))]
public class Contract
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int BillingId { get; set; }
    [Required]
    public int EntityId { get; set; }
    [Required]
    [Column(TypeName = "bit")]
    public bool Signed { get; set; }
    [Required]
    [Column(TypeName = "date")]
    public DateTime From { get; set; }
    [Required]
    [Column(TypeName = "date")]
    public DateTime To { get; set; }

    [ForeignKey(nameof(BillingId))]
    public virtual Billing Billing { get; set; } = null!;

    [ForeignKey(nameof(EntityId))]
    public virtual Entity Entity { get; set; } = null!;
    
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<AvailableVersion> AvailableVersions { get; set; } = new List<AvailableVersion>();
}