namespace API.DTOs.BusinessDTOs;

public record OutAddrDto(
    string Country,
    string City,
    string Street,
    string PostalCode,
    int BuildingNumber,
    int? ApartmentNumber
    )
{
    
}