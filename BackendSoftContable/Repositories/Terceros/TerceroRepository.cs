using BackendSoftContable.Data;
using BackendSoftContable.Models.Terceros;
using BackendSoftContable.Repositories.ITerceroRepositories;
using Microsoft.EntityFrameworkCore;

namespace BackendSoftContable.Repositories.TerceroRepository
{
    public class TerceroRepository : ITerceroRepository
    {
        private readonly AppDbContext _context;
        public TerceroRepository(AppDbContext context) => _context = context;

        public async Task<Tercero?> GetByIdentificacionAsync(string identificacion)
            => await _context.Tercero.FirstOrDefaultAsync(t => t.Identificacion == identificacion);

        public async Task<Tercero?> GetByIdAsync(Guid id)
            => await _context.Tercero.FirstOrDefaultAsync(t => t.Id == id);

        public async Task AddAsync(Tercero tercero)
        {
            await _context.Tercero.AddAsync(tercero);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tercero tercero)
        {
            _context.Tercero.Update(tercero);
            await _context.SaveChangesAsync();
        }
    }
}