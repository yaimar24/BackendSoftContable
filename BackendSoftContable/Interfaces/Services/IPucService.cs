using BackendSoftContable.DTOs.Puc;
using BackendSoftContable.DTOs;

public interface IPucService
{
    Task<ApiResponseDTO<IEnumerable<PucNodoDTO>>> GetTreeAsync(Guid colegioId);
    Task<ApiResponseDTO<string>> CreateAccountAsync(PucCreateDTO dto, Guid colegioId, Guid usuarioId);
}