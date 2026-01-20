using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Login;
using BackendSoftContable.DTOs.Auditoria;
using BackendSoftContable.Data.Repositories;
using BackendSoftContable.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace BackendSoftContable.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly IAuditService _auditService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepo,
        IPasswordService passwordService,
        IJwtService jwtService,
        IAuditService auditService,
        ILogger<AuthService> logger)
    {
        _usuarioRepo = usuarioRepo;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<ApiResponseDTO<LoginResponseDTO>> LoginAsync(LoginDTO dto)
    {
        var usuario = await _usuarioRepo.GetByEmailAsync(dto.Email);

        // 1️⃣ Validación de credenciales
        if (usuario == null || !_passwordService.Verify(usuario.PasswordHash, dto.Password))
        {
            if (usuario != null)
            {
                await SafeAuditAsync(
                    usuario.Id,
                    usuario.ColegioId,
                    false,
                    "Login fallido: Clave incorrecta"
                );
            }

            return new ApiResponseDTO<LoginResponseDTO>
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        // 2️⃣ Generar JWT
        var token = _jwtService.GenerateToken(
            usuario,
            usuario.Colegio?.NombreColegio ?? "SinColegio"
        );

        // 3️⃣ Auditoría de éxito
        await SafeAuditAsync(
            usuario.Id,
            usuario.ColegioId,
            true,
            "Login exitoso"
        );

        return new ApiResponseDTO<LoginResponseDTO>
        {
            Success = true,
            Message = "Login exitoso",
            Data = new LoginResponseDTO
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1)
            }
        };
    }

    /// <summary>
    /// Ejecuta la auditoría de forma segura sin romper el flujo principal
    /// </summary>
    private async Task SafeAuditAsync(
        Guid usuarioId,
        Guid colegioId,
        bool exitoso,
        string descripcion)
    {
        try
        {
            var entry = new AuditEntry
            {
                Accion = "LOGIN",
                Modulo = "Seguridad",
                Entidad = "Usuario",
                Descripcion = descripcion,
                DatosDespues = new { Resultado = exitoso ? "Exitoso" : "Fallido" },
                Exitoso = exitoso
            };

            var ctx = new AuditContext
            {
                UsuarioId = usuarioId,
                ColegioId = colegioId
            };

            await _auditService.LogAsync(entry, ctx);
        }
        catch (Exception ex)
        {
            // Nunca rompe el login
            _logger.LogError(
                ex,
                "Error persistiendo auditoría LOGIN. Usuario {UsuarioId}, Colegio {ColegioId}",
                usuarioId,
                colegioId
            );
        }
    }
}
