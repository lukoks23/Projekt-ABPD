using Microsoft.AspNetCore.Identity;

namespace API.Services;

public class PasswordService : IPasswordService
{
    PasswordHasher<object> _hasher = new(); // Klasa z funkcjonalnością hasującą pochodząca z platformy Identity.
                                            // Bezpieczna, rozszerzalna. Automatycznie tworzy hash z doklejeniem soli do wartości.
                                            // Wartość soli przechowywana jest w hashu.
    
    public string HashPassword(string password)
    {
        return _hasher.HashPassword(null, password);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        return _hasher.VerifyHashedPassword(null, hashedPassword, providedPassword) != PasswordVerificationResult.Failed;
    }
}