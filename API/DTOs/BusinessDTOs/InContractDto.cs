using System.ComponentModel.DataAnnotations;

namespace API.DTOs.BusinessDTOs;

public record InContractDto(
    [MaxLength(11)][MinLength(11)] string? Pesel,
    [MaxLength(10)][MinLength(10)] string? Krs,
    [Required] DateTime DateFrom,
    [Required] DateTime DateTo,
    [Required] string BillingType,
    [Required] int YearsOfSupport,
    [Required] int SoftwareId,
    [Required] ICollection<int> VersionsIds
    )
{
    
}