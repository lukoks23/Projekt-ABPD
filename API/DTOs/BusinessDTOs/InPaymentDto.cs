using System.ComponentModel.DataAnnotations;

namespace API.DTOs.BusinessDTOs;

public record InPaymentDto(
    [Required]DateTime PaymentDate,
    [Required]decimal TransferredAmount,
    [Required]int ContractId,
    [Required][MaxLength(34)]string BankAccountNumber
    )
{
    
}