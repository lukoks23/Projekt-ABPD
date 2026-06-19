using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class SignInRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}