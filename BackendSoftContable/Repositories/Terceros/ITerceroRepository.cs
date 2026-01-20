using BackendSoftContable.Data;
using BackendSoftContable.Models.Terceros;
using BackendSoftContable.Models;
public interface ITerceroRepository
{
    Task<Tercero?> GetByIdentificacionCompletoAsync(string identificacion);
    Task<Tercero?> GetByIdCompletoAsync(Guid id);
    Task<TerceroCategoria?> GetVinculacionAsync(Guid terceroId, Guid colegioId);
    Task<IEnumerable<TerceroCategoria>> GetTodosPorColegioAsync(Guid colegioId);
    Task AddAsync(Tercero tercero);
    Task AddVinculacionAsync(TerceroCategoria vinculacion);
    void RemoveResponsabilidades(IEnumerable<TerceroResponsabilidad> responsabilidades);
    Task SaveChangesAsync();
}
