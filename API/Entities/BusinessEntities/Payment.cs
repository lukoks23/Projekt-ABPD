using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities.BusinessEntities;

[Table("Payments")]
public class Payment
{
    [Key]
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "date")]
    public DateTime PaymentDate { get; set; }
    [Required]
    [Column(TypeName = "decimal(15,2)")]
    public decimal TransferredMoney { get; set; }
    public int ContractId { get; set; }
    [Column(TypeName = "nvarchar(34)")]
    public string BankAccountNumber { get; set; } = string.Empty;
    
    [ForeignKey("ContractId")]
    public virtual Contract Contract { get; set; } = null!;
}