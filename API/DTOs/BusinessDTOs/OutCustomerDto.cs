namespace API.DTOs.BusinessDTOs;

public record OutCustomerDto(
    OutAddrDto Address,
    OutCompanyDto? Company,
    OutInvidualDto? Invidual
    )
{
    
}