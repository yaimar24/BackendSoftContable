using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Models;

namespace BackendSoftContable.Data.Repositories
{
    public class ColegioRepository : IColegioRepository
    {
        private readonly AppDbContext _context;

        public ColegioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Colegio> AddAsync(Colegio colegio)
        {
            _context.Colegios.Add(colegio);
            await _context.SaveChangesAsync();
            return colegio;
        }

        public async Task<Colegio?> GetByIdAsync(int id)
        {
            return await _context.Colegios
                                 .Include(c => c.Usuarios)
                                 .Include(c => c.RepresentantesLegales) 
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Colegio>> GetAllAsync()
        {
            return await _context.Colegios
                                 .Include(c => c.Usuarios)
                                 .ToListAsync();
        


    }

        public async Task<bool> ExistsByNitAsync(string nit)
        {
            return await _context.Colegios.AnyAsync(c => c.Nit == nit);
        }

        public async Task UpdateAsync(Colegio colegio)
        {
            _context.Colegios.Update(colegio);
            await _context.SaveChangesAsync();
        }

    }
}
