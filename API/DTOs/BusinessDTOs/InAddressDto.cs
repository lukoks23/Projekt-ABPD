using System.ComponentModel.DataAnnotations;

namespace API.DTOs.BusinessDTOs;

public record InAddressDto(
    [Required][MaxLength(100)] string Country,    
    [Required][MaxLength(100)] string City,
    [Required][MaxLength(100)] string Street,
    [Required] int BuildingNumber,
    int? ApartmentNumber,
    [Required][MaxLength(15)] string PostalCode
    )
{
    
}