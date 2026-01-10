using BackendSoftContable.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(Usuario usuario, string nombreColegio)
    {
        var jwtSection = _config.GetSection("Jwt");
        string key = jwtSection["Key"] ?? throw new ArgumentNullException("JWT Key is missing in configuration");
        string issuer = jwtSection["Issuer"] ?? throw new ArgumentNullException("JWT Issuer is missing in configuration");
        string audience = jwtSection["Audience"] ?? throw new ArgumentNullException("JWT Audience is missing in configuration");
        int expireMinutes = int.Parse(jwtSection["ExpireMinutes"] ?? "60");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim("nombreColegio", nombreColegio),
            new Claim("colegioId", usuario.ColegioId.ToString()),
            new Claim(ClaimTypes.Role, usuario.Roles.Nombre) 
        };

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
