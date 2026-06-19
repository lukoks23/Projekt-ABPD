using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Licenses"),PrimaryKey(nameof(Id))]
public class License
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int YearsOfSupport { get; set; }
    [Required]
    public decimal FinalPrice { get; set; }
    
    public virtual Billing? Billing { get; set; }
}