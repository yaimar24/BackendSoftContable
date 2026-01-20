using BackendSoftContable.Data;
using BackendSoftContable.Models;
using BackendSoftContable.Models.Terceros;
using Microsoft.EntityFrameworkCore;

namespace BackendSoftContable.Repositories.TerceroRepository
{
 
    public class TerceroRepository : ITerceroRepository
    {
        private readonly AppDbContext _context;
        public TerceroRepository(AppDbContext context) => _context = context;

        public async Task<Tercero?> GetByIdentificacionCompletoAsync(string identificacion) =>
            await _context.Tercero
                .Include(t => t.InformacionFiscal)
                .Include(t => t.Responsabilidades)
                .FirstOrDefaultAsync(t => t.Identificacion == identificacion);

        public async Task<Tercero?> GetByIdCompletoAsync(Guid id) =>
            await _context.Tercero
                .Include(t => t.InformacionFiscal)
                .Include(t => t.Responsabilidades)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<TerceroCategoria?> GetVinculacionAsync(Guid terceroId, Guid colegioId) =>
            await _context.TerceroCategoria
                .FirstOrDefaultAsync(tc => tc.TerceroId == terceroId && tc.ColegioId == colegioId);

        public async Task<IEnumerable<TerceroCategoria>> GetTodosPorColegioAsync(Guid colegioId) =>
            await _context.TerceroCategoria
                .AsNoTracking()
                .Where(tc => tc.ColegioId == colegioId)
                .Include(tc => tc.Tercero).ThenInclude(t => t.InformacionFiscal)
                .Include(tc => tc.Tercero).ThenInclude(t => t.Responsabilidades)
                .ToListAsync();

        public async Task AddAsync(Tercero tercero) => await _context.Tercero.AddAsync(tercero);

        public async Task AddVinculacionAsync(TerceroCategoria vinculacion) =>
            await _context.TerceroCategoria.AddAsync(vinculacion);

        public void RemoveResponsabilidades(IEnumerable<TerceroResponsabilidad> responsabilidades) =>
            _context.TerceroResponsabilidad.RemoveRange(responsabilidades);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}