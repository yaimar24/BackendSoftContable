using BackendSoftContable.DTOs;

namespace BackendSoftContable.Services
{
    public interface IColegioService
    {
        Task<ColegioDTO> RegisterColegioAsync(ColegioCreateDTO dto); 
        Task<ColegioDTO?> GetColegioByIdAsync(int id);
    }
}