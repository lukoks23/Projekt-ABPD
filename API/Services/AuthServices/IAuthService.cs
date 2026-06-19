using API.DTOs;
using Microsoft.AspNetCore.Identity.Data;

namespace API.Services;

public interface IAuthService
{
    Task<SignResp> LoginAsync(SignInRequest request, CancellationToken ct);
    Task<SignResp> RegisterAsync(SignUpRequest request, CancellationToken ct);
    Task LogOutAsync(string? refreshToken, CancellationToken ct);
    Task<SignResp> RefreshTokenAsync(string? refreshToken, CancellationToken ct);
}