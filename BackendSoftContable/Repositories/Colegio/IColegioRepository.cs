using BackendSoftContable.Models;

namespace BackendSoftContable.Data.Repositories
{
    public interface IColegioRepository
    {
        Task<Colegio> AddAsync(Colegio colegio);
        Task<Colegio?> GetByIdAsync(int id);
        Task<IEnumerable<Colegio>> GetAllAsync();

        Task<bool> ExistsByNitAsync(string nit);

        Task UpdateAsync(Colegio colegio);

    }
}
