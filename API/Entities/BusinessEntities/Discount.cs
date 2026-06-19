using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Discounts"),PrimaryKey(nameof(Id))]
public class Discount
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string Name { get; set; } = string.Empty;
    [Required]
    public decimal Percent { get; set; }
    [Required]
    [Column(TypeName = "date")]
    public DateTime From { get; set; }
    [Required]
    [Column(TypeName = "date")]
    public DateTime To { get; set; }
    [Required]
    public int BillingTypeId { get; set; }

    [ForeignKey(nameof(BillingTypeId))]
    public virtual BillingType BillingType { get; set; } = null!;

    public virtual ICollection<SoftDisc> SoftDiscs { get; set; } = new List<SoftDisc>();
}