using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Colegio;

public interface IColegioService
{
    Task<ApiResponseDTO<ColegioDTO>> RegisterAsync(ColegioCreateDTO dto);
    Task<ColegioDetailDTO?> GetByIdAsync(Guid id);
    // Añadimos el ID del usuario que opera
    Task<ApiResponseDTO<bool>> UpdateAsync(ColegioUpdateDTO dto, Guid usuarioId);
}