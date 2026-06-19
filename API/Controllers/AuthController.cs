using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request, CancellationToken ct)
    {
        var ans = await authService.LoginAsync(request, ct);
        AppendRefreshTokenCookie(HttpContext, ans.RefreshToken);
        return Ok(new { ans.AccessToken });
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUpAsync([FromBody] SignUpRequest request, CancellationToken ct)
    {
        var ans = await authService.RegisterAsync(request, ct);
        AppendRefreshTokenCookie(HttpContext, ans.RefreshToken);
        return Ok(new { ans.AccessToken });
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync(CancellationToken ct)
    {
        authService.LogOutAsync(GetRefreshTokenCookieValue(HttpContext),ct);
        RemoveRefreshTokenCookie(HttpContext);
        return Ok();
    }

    // Proces refreshowania
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync(CancellationToken ct)
    {
        var ans = await authService.RefreshTokenAsync(GetRefreshTokenCookieValue(HttpContext),ct);
        AppendRefreshTokenCookie(HttpContext, ans.RefreshToken);
        return Ok(new { ans.AccessToken });
    }
    
    // Helper do dodania do odpowiedzi ciasteczka Strict HttpOnly zawierającego wartość refresh-tokenu użytkownika
    private static void AppendRefreshTokenCookie(HttpContext httpContext, string refreshToken)
    {
        httpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(7),
            SameSite = SameSiteMode.Strict
        });
    }

    // Helper do pobierania wartości ciasteczka zawierającego refresh-token
    private static string? GetRefreshTokenCookieValue(HttpContext httpContext)
    {
        return httpContext.Request.Cookies["refreshToken"];
    }

    // Helper do usuwania ciasteczka zawierającego refresh-token
    private static void RemoveRefreshTokenCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
    }
}