using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities.BusinessEntities;

[Table("Billings"),PrimaryKey(nameof(Id))]
public class Billing
{
    [Key]
    public int Id { get; set; }
    public int? SubscriptionId { get; set; }
    public int? LicenceId { get; set; }
    
    [ForeignKey(nameof(LicenceId))]
    public virtual License? License { get; set; }
    [ForeignKey(nameof(SubscriptionId))]
    public virtual Subscription? Subscription { get; set; }

    public virtual Contract Contract { get; set; } = null!;
}