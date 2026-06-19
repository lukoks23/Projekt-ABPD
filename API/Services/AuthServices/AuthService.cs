using API.DTOs;
using API.Entities;
using API.Exceptions;
using API.Infrastructure;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AuthService(DatabaseContext ctx, ITokenService tokenService, IPasswordService passwordService) : IAuthService
{
    public async Task<SignResp> LoginAsync(SignInRequest request, CancellationToken ct)
    {
        var user = await ctx.Users.Include(e => e.Role).FirstOrDefaultAsync(e => e.UserName == request.Username.Trim().ToLowerInvariant(),ct);
        if (user is null || !passwordService.VerifyHashedPassword(user.PasswordHash, request.Password))
        {
            throw new UnauthorizedException("Incorrect username or password");
        }
        
        var accessToken = tokenService.GenerateToken(user.Id.ToString(), user.UserName, user.Role.Name);
        var refreshToken = tokenService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.ExpiresAt = DateTime.UtcNow.AddDays(7);
        
        await ctx.SaveChangesAsync(ct);
        return new SignResp(accessToken, refreshToken);
    }

    public async Task<SignResp> RegisterAsync(SignUpRequest request, CancellationToken ct)
    {
        if (await ctx.Users.AnyAsync(e => e.UserName == request.Username.Trim().ToLowerInvariant(),ct))
        {
            throw new ConflictException("Username is already taken");
        }
        var passwordHash = passwordService.HashPassword(request.Password);
        var worker = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "worker",ct);
        if (worker is null)
        {
            throw new NotFoundException("Worker role not found");
        }
        var user = new User
        {
            UserName = request.Username.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = worker
        };
        
        var accessToken = tokenService.GenerateToken(user.Id.ToString(), user.UserName, user.Role.Name);
        var refreshToken = tokenService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.ExpiresAt = DateTime.UtcNow.AddDays(7);
        
        await ctx.Users.AddAsync(user,ct);
        await ctx.SaveChangesAsync(ct);
        return new SignResp(accessToken, refreshToken);
    }

    public async Task LogOutAsync(string? refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedException("Invalid refresh token");
        }
        
        var user = await ctx.Users.FirstOrDefaultAsync(e => e.RefreshToken == refreshToken,ct);
        if (user is null)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        user.RefreshToken = null;
        await ctx.SaveChangesAsync(ct);
    }

    public async Task<SignResp> RefreshTokenAsync(string? refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedException("Invalid refresh token");
        }
        
        var user = await ctx.Users.Include(e => e.Role).FirstOrDefaultAsync(e => e.RefreshToken == refreshToken && e.ExpiresAt >= DateTime.UtcNow,ct);
        if (user is null)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }
        
        var accessToken = tokenService.GenerateToken(user.Id.ToString(), user.UserName, user.Role.Name);
        var newRefreshToken = tokenService.GenerateRefreshToken();
        
        // 4. Zapisywanie refresh tokenu dla użytkownika i ustawienie czasu jego "życia"
        user.RefreshToken = newRefreshToken;
        user.ExpiresAt = DateTime.UtcNow.AddDays(7);
        await ctx.SaveChangesAsync(ct);
        
        return new SignResp(accessToken, newRefreshToken);
    }
}