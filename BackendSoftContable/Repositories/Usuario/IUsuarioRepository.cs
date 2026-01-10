using BackendSoftContable.Models;
using System;

namespace BackendSoftContable.Data.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> AddAsync(Usuario usuario);
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetAllByColegioAsync(int colegioId);

        Task<bool> ExistsByEmailAsync(string email);


    }


}