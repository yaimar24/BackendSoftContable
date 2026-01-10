using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Login;
using BackendSoftContable.Services.Security;
using BackendSoftContable.Data.Repositories;

namespace BackendSoftContable.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUsuarioRepository usuarioRepo,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _usuarioRepo = usuarioRepo;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<ApiResponseDTO<LoginResponseDTO>> LoginAsync(LoginDTO dto)
    {
        // Buscar usuario por email
        var usuario = await _usuarioRepo.GetByEmailAsync(dto.Email);

        // Validar usuario y contraseña
        if (usuario == null ||
            !_passwordService.Verify(usuario.PasswordHash, dto.Password))
        {
            return new ApiResponseDTO<LoginResponseDTO>
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        // Generar token JWT
        var token = _jwtService.GenerateToken(
            usuario,
            usuario.Colegio?.NombreColegio ?? "SinColegio"
        );

        // Retornar respuesta con token y expiración
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
}
