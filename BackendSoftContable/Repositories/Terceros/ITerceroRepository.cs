using BackendSoftContable.Models.Terceros;

namespace BackendSoftContable.Repositories.ITerceroRepositories
{
 
        public interface ITerceroRepository
        {
            Task<Tercero?> GetByIdentificacionAsync(string identificacion);
            Task AddAsync(Tercero tercero);
            Task<Tercero?> GetByIdAsync(Guid id);
        }
    
}
