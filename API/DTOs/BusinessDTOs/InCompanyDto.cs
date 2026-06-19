using System.ComponentModel.DataAnnotations;

namespace API.DTOs.BusinessDTOs;

public record InCompanyDto(
    [Required][MaxLength(10)][MinLength(10)] string Krs,
    [Required][MaxLength(100)][EmailAddress] string Email,
    [Required][MaxLength(16)] string Phone
    )
{
    
}