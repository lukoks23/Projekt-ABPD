using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("SoftDiscs"),PrimaryKey(nameof(Id))]
public class SoftDisc
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int SoftwareId { get; set; }
    [Required]
    public int DiscountId { get; set; }

    [ForeignKey(nameof(DiscountId))]
    public virtual Discount Discount { get; set; } = null!;

    [ForeignKey(nameof(SoftwareId))]
    public virtual Software Software { get; set; } = null!;
}