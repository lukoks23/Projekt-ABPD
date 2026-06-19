namespace API.DTOs.BusinessDTOs;

public record InUpdateCustomerDto(
    InAddressDto? Address,
    InUpdateInvidualDto? Individual,
    InUpdateCompanyDto? Company
    )
{
    
}