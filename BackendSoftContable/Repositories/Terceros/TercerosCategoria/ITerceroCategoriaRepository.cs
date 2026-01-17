using BackendSoftContable.Models;

namespace BackendSoftContable.Repositories.ITercerosCategoria
{
    public interface ITerceroCategoriaRepository
    {
        Task<bool> ExistsAsync(Guid terceroId, Guid colegioId, int categoriaId);
        Task AddAsync(TerceroCategoria categoria);
        Task<IEnumerable<TerceroCategoria>> GetByColegioAsync(Guid colegioId);
    }

}
