using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Data;
using BackendSoftContable.DTOs;

namespace BackendSoftContable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParametrosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("parametros")]
        public async Task<IActionResult> GetParametrosRegistro()
        {
            // Ejecutamos una por una con 'await' para respetar el hilo del DbContext
            var actividades = await _context.ActividadEconomica
                .Select(a => new { a.Id, a.Codigo, a.Descripcion }).ToListAsync();

            var regimenes = await _context.RegimenesIva
                .Where(r => r.Activo)
                .Select(r => new { r.Id, r.Nombre }).ToListAsync();

            var responsabilidades = await _context.ResponsabilidadFiscal
                .Select(r => new { r.Id, r.Nombre }).ToListAsync();

            var tiposId = await _context.TipoIdentificacion
                .Select(t => new { t.Id, t.Nombre }).ToListAsync();

            var ciudades = await _context.Ciudad
                .Select(c => new { c.Id, c.Nombre }).ToListAsync();

            var tributos = await _context.Tributo
                .Select(t => new { t.Id, t.Nombre }).ToListAsync();
            var tiposPersona = await _context.TipoPersona
                    .Select(t => new { t.Id, t.Nombre }).ToListAsync();

            var categorias = await _context.Categorias
                .Where(c => c.Activo)
                .Select(c => new { c.Id, c.Nombre }).ToListAsync();

            return Ok(new RegistroParametrosDTO
            {
                ActividadesEconomicas = actividades,
                RegimenesIva = regimenes,
                ResponsabilidadesFiscales = responsabilidades,
                TiposIdentificacion = tiposId,
                Ciudades = ciudades,
                Tributos = tributos,
                Categorias = categorias,
                TiposPersona = tiposPersona
            });
        }
    }
}