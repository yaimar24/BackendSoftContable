using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Login;

public interface IAuthService
{
    Task<ApiResponseDTO<LoginResponseDTO>> LoginAsync(LoginDTO dto);
}
