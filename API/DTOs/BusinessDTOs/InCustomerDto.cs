using System.ComponentModel.DataAnnotations;

namespace API.DTOs.BusinessDTOs;

public record InCustomerDto(
    [Required] InAddressDto Address,
    InInvidualDto? Individual,
    InCompanyDto? Company
    )
{
    
}