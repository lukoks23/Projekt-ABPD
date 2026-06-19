using System.ComponentModel.DataAnnotations;

namespace API.DTOs.BusinessDTOs;

public record InUpdateInvidualDto(
    [Required][MaxLength(11)][MinLength(11)] string Pesel,
    [MaxLength(100)] string? FirstName,
    [MaxLength(100)] string? LastName,
    [EmailAddress] string? Email,
    [MaxLength(16)] string? Phone
    );