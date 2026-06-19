using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Subscriptions"),PrimaryKey(nameof(Id))]
public class Subscription
{
    [Key]
    public int Id { get; set; }
    
    public virtual Billing? Billing { get; set; }
}