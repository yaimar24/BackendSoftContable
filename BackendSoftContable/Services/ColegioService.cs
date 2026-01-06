using BackendSoftContable.Data.Repositories;
using BackendSoftContable.DTOs;
using BackendSoftContable.Models;
using System.Security.Cryptography;
using System.Text;

namespace BackendSoftContable.Services
{
    public class ColegioService : IColegioService
    {
        private readonly IColegioRepository _repo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IWebHostEnvironment _env;

        public ColegioService(IColegioRepository repo, IUsuarioRepository usuarioRepo, IWebHostEnvironment env)
        {
            _repo = repo;
            _usuarioRepo = usuarioRepo;
            _env = env;
        }

        public async Task<ColegioDTO> RegisterColegioAsync(ColegioCreateDTO dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Las contraseñas no coinciden");

            // Crear entidad Colegio
            var colegio = new Colegio
            {
                NombreColegio = dto.NombreColegio,
                Nit = dto.Nit,
                Direccion = dto.Direccion,
                ResDian = dto.ResDian,
                FechaCertificado = dto.FechaCertificado,
                RepresentanteLegal = dto.RepresentanteLegal,
                Cedula = dto.Cedula,
                PlanSeleccionado = dto.PlanSeleccionado,
                LogoPath = dto.Logo != null ? await SaveFileAsync(dto.Logo) : null,
                ArchivoDianPath = dto.ArchivoDian != null ? await SaveFileAsync(dto.ArchivoDian) : null,
                Usuarios = new List<Usuario>()
            };

            // Guardar Colegio primero
            var result = await _repo.AddAsync(colegio);

            // Crear usuario administrador
            var admin = new Usuario
            {
                Nombre = "Administrador",
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Rol = "admin",
                ColegioId = result.Id
            };

            await _usuarioRepo.AddAsync(admin);

            // Mapear DTO
            return new ColegioDTO
            {
                Id = result.Id,
                NombreColegio = result.NombreColegio,
                Nit = result.Nit,
                Direccion = result.Direccion,
                PlanSeleccionado = result.PlanSeleccionado,
                LogoPath = result.LogoPath,
                ArchivoDianPath = result.ArchivoDianPath
            };
        }

        public async Task<ColegioDTO?> GetColegioByIdAsync(int id)
        {
            var colegio = await _repo.GetByIdAsync(id);
            if (colegio == null) return null;

            return new ColegioDTO
            {
                Id = colegio.Id,
                NombreColegio = colegio.NombreColegio,
                Nit = colegio.Nit,
                Direccion = colegio.Direccion,
                PlanSeleccionado = colegio.PlanSeleccionado,
                LogoPath = colegio.LogoPath,
                ArchivoDianPath = colegio.ArchivoDianPath
            };
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            Directory.CreateDirectory(uploads);
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var path = Path.Combine(uploads, fileName);

            await using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }
    }
}
