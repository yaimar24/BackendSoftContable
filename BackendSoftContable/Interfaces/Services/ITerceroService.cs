using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Tercero;
using BackendSoftContable.DTOs.Terceros;
using BackendSoftContable.DTOs.TerceroUpdateDTO;

public interface ITerceroService
{
    // Crear un tercero y vincularlo a un colegio
    Task<ApiResponseDTO<Guid>> CreateWithCategoryAsync(TerceroCreateDTO dto, Guid usuarioId);

    // Actualizar un tercero existente
    Task<ApiResponseDTO<Guid>> UpdateAsync(TerceroEditDTO dto, Guid usuarioId);

    // Obtener todos los terceros de un colegio
    Task<ApiResponseDTO<IEnumerable<TerceroEditDTO>>> ObtenerTodosPorColegio(Guid colegioId);

    Task<ApiResponseDTO<Guid>> DesvincularTerceroAsync(Guid terceroId, Guid colegioId, Guid usuarioId);


}
