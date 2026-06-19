namespace API.Services;

public interface ITokenService
{
    string GenerateToken(string userId, string username, string userRole);
    string GenerateRefreshToken();
}