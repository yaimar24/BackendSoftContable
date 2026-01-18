using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Login;
using BackendSoftContable.Services.Security;
using BackendSoftContable.Data.Repositories;
using BackendSoftContable.DTOs.Auditoria;
using BackendSoftContable.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BackendSoftContable.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly IServiceScopeFactory _scopeFactory;

    public AuthService(
        IUsuarioRepository usuarioRepo,
        IPasswordService passwordService,
        IJwtService jwtService,
        IServiceScopeFactory scopeFactory)
    {
        _usuarioRepo = usuarioRepo;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _scopeFactory = scopeFactory;
    }

    public async Task<ApiResponseDTO<LoginResponseDTO>> LoginAsync(LoginDTO dto)
    {
        var usuario = await _usuarioRepo.GetByEmailAsync(dto.Email);

        // Validación de credenciales
        if (usuario == null || !_passwordService.Verify(usuario.PasswordHash, dto.Password))
        {
            if (usuario != null)
            {
                // Si el error decía que no existe GetValueOrDefault(), 
                // entonces pasamos el Id directamente porque no es nulo.
                AuditarLogin(usuario.Id, usuario.ColegioId, false, "Login fallido: Clave incorrecta");
            }

            return new ApiResponseDTO<LoginResponseDTO>
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        var token = _jwtService.GenerateToken(
            usuario,
            usuario.Colegio?.NombreColegio ?? "SinColegio"
        );

        // Auditoría de éxito
        AuditarLogin(usuario.Id, usuario.ColegioId, true, "Login exitoso");

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

    private void AuditarLogin(Guid usuarioId, Guid colegioId, bool exitoso, string descripcion)
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
            ColegioId = colegioId,
            Endpoint = "/api/auth/login",
            MetodoHttp = "POST"
        };

        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                await auditService.LogAsync(entry, ctx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error Auditoría: {ex.Message}");
            }
        });
    }
}