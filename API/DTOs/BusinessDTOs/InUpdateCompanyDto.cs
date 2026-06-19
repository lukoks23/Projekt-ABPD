using System.ComponentModel.DataAnnotations;

namespace API.DTOs.BusinessDTOs;

public record InUpdateCompanyDto(
    [Required][MaxLength(10)][MinLength(10)] string Krs,
    [MaxLength(100)][EmailAddress] string? Email,
    [MaxLength(16)] string? Phone
    )
{
    
}