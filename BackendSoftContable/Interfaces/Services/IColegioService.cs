using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Colegio;

public interface IColegioService
{
    Task<ApiResponseDTO<ColegioDTO>> RegisterAsync(ColegioCreateDTO dto);
    Task<ColegioDetailDTO?> GetByIdAsync(int id);
    Task<ApiResponseDTO<bool>> UpdateAsync(ColegioUpdateDTO dto);
}
