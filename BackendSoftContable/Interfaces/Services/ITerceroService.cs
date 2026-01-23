using BackendSoftContable.DTOs.Terceros;
using BackendSoftContable.DTOs;

namespace BackendSoftContable.Interfaces.Services
{
    public interface ITerceroService
    {
        /// <summary>
        /// Crea un nuevo tercero o vincula uno existente a un colegio específico.
        /// </summary>
        Task<ApiResponseDTO<Guid>> CreateWithCategoryAsync(TerceroCreateDTO dto, Guid usuarioId);

        /// <summary>
        /// Actualiza la información fiscal y los datos de vinculación de un tercero.
        /// </summary>
        Task<ApiResponseDTO<Guid>> UpdateAsync(TerceroEditDTO dto, Guid usuarioId);

        /// <summary>
        /// Recupera la lista completa de terceros asociados a un colegio con sus datos fiscales.
        /// </summary>
        Task<ApiResponseDTO<IEnumerable<TerceroEditDTO>>> ObtenerTodosPorColegio(Guid colegioId);

        /// <summary>
        /// Cambia el estado de activación (Activo/Inactivo) de un tercero en un colegio.
        /// </summary>
        Task<ApiResponseDTO<Guid>> DesvincularTerceroAsync(Guid terceroId, Guid colegioId, Guid usuarioId);

        /// <summary>
        /// Obtiene todos los clientes vinculados a un colegio, opcionalmente filtrando por nombre.
        /// </summary>
        Task<ApiResponseDTO<List<TerceroClienteDTO>>> GetClientesAsync(Guid colegioId, string? nombreFiltro = null);
    }
}
