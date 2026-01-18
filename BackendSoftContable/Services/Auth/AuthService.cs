using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Login;
using BackendSoftContable.Data.Repositories;
using BackendSoftContable.DTOs.Auditoria;
using BackendSoftContable.Interfaces.Services;


namespace BackendSoftContable.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuthService> _logger; 
    public AuthService(
        IUsuarioRepository usuarioRepo,
        IPasswordService passwordService,
        IJwtService jwtService,
        IServiceScopeFactory scopeFactory,
        ILogger<AuthService> logger)
    {
        _usuarioRepo = usuarioRepo;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<ApiResponseDTO<LoginResponseDTO>> LoginAsync(LoginDTO dto)
    {
        var usuario = await _usuarioRepo.GetByEmailAsync(dto.Email);

        // 1. Validación de credenciales
        if (usuario == null || !_passwordService.Verify(usuario.PasswordHash, dto.Password))
        {
            if (usuario != null)
            {
                // Auditoría de intento fallido
                AuditarLogin(usuario.Id, usuario.ColegioId, false, "Login fallido: Clave incorrecta");
            }

            return new ApiResponseDTO<LoginResponseDTO>
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        // 2. Generación de Token
        var token = _jwtService.GenerateToken(
            usuario,
            usuario.Colegio?.NombreColegio ?? "SinColegio"
        );

        // 3. Auditoría de éxito
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
        // Preparamos los datos capturando los valores antes del Task.Run
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
        };

        // Ejecución en segundo plano para no retrasar el Login al usuario
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
                // LOG DE PRODUCCIÓN: 
                // Esto permite rastrear fallos en el visor de eventos o logs del servidor
                _logger.LogError(ex, "Error crítico persistiendo auditoría de LOGIN para usuario {UsuarioId} en colegio {ColegioId}", usuarioId, colegioId);
            }
        });
    }
}