using BackendSoftContable.Models;
using Microsoft.AspNetCore.Identity;

namespace BackendSoftContable.Services.Security;


public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<Usuario> _hasher = new();

    // Para registrar
    public string Hash(string password)
    {
        var tempUser = new Usuario(); // Usuario temporal para generar hash
        return _hasher.HashPassword(tempUser, password);
    }

    // Para login / verificación
    public bool Verify(string hash, string password)
    {
        var tempUser = new Usuario(); // Usuario temporal para verificar hash
        return _hasher.VerifyHashedPassword(tempUser, hash, password)
               == PasswordVerificationResult.Success;
    }
}

