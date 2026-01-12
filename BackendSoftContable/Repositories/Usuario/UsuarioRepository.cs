using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Models;

namespace BackendSoftContable.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> GetByIdAsync(Guid id)
        {
            return await _context.Usuarios
                                 .Include(u => u.Colegio)
                                 .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                                 .Include(u => u.Colegio)
                                 .Include(u => u.Roles)
                                 .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<IEnumerable<Usuario>> GetAllByColegioAsync(Guid colegioId)
        {
            return await _context.Usuarios
                                 .Where(u => u.ColegioId == colegioId)
                                 .ToListAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

    }
}
