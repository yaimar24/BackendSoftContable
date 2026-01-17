using BackendSoftContable.Data;
using BackendSoftContable.Models;
using BackendSoftContable.Repositories.ITercerosCategoria;
using Microsoft.EntityFrameworkCore;

namespace BackendSoftContable.Repositories.TerceroCategoriaRepository
{
    public class TerceroCategoriaRepository : ITerceroCategoriaRepository
    {
        private readonly AppDbContext _context;
        public TerceroCategoriaRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<TerceroCategoria>> GetByColegioAsync(Guid colegioId)
        {
            return await _context.TerceroCategoria
                .Include(tc => tc.Tercero)
                .Include(tc => tc.RegimenIva)
                .Where(tc => tc.ColegioId == colegioId)
                .ToListAsync();
        }

        public async Task AddAsync(TerceroCategoria categoria)
        {
            await _context.TerceroCategoria.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid terceroId, Guid colegioId, int categoriaId)
                    => await _context.TerceroCategoria
    .AnyAsync(tc => tc.TerceroId == terceroId
                   && tc.ColegioId == colegioId
                   && tc.CategoriaId == categoriaId);
    }
}
