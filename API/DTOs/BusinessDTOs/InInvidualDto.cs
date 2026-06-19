using System.ComponentModel.DataAnnotations;

namespace API.DTOs.BusinessDTOs;

public record InInvidualDto(
    [Required][MaxLength(11)][MinLength(11)] string Pesel,
    [Required][MaxLength(100)] string FirstName,
    [Required][MaxLength(100)] string LastName,
    [Required][EmailAddress] string Email,
    [Required][MaxLength(16)] string Phone
    )
{
    
}