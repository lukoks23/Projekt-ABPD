using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("BillingTypes"),PrimaryKey(nameof(Id))]
public class BillingType
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string Type { get; set; } = string.Empty;
    
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
    public virtual ICollection<SoftwareCost> SoftwareCosts { get; set; } = new List<SoftwareCost>();
}