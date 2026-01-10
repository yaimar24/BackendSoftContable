using BackendSoftContable.Models;

public interface IJwtService
{
    string GenerateToken(Usuario usuario, string nombreColegio);
}