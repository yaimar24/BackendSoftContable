using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.TerceroDetalleDTO;


    public interface ITerceroService
    {
    Task<ApiResponseDTO<Guid>> CreateWithCategoryAsync(TerceroCreateDTO dto, Guid usuarioId);

    Task<ApiResponseDTO<IEnumerable<TerceroDetalleDTO>>> GetByColegioAsync(
            Guid colegioId);
   
}
