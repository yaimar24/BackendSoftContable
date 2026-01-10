using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Data;
using BackendSoftContable.Models;

namespace BackendSoftContable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegimenIvaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegimenIvaController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos los regímenes IVA activos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regimenes = await _context.RegimenesIva
                .Where(r => r.Activo)
                .Select(r => new
                {
                    r.Id,
                    r.Nombre
                })
                .ToListAsync();

            return Ok(regimenes);
        }

        /// <summary>
        /// Obtiene un régimen IVA por Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var regimen = await _context.RegimenesIva
                .Where(r => r.Activo && r.Id == id)
                .Select(r => new
                {
                    r.Id,
                    r.Nombre,
                    r.Descripcion
                })
                .FirstOrDefaultAsync();

            if (regimen == null)
                return NotFound("Régimen IVA no encontrado");

            return Ok(regimen);
        }
    }
}
