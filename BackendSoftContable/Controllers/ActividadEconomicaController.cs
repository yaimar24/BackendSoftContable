using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Data;
using BackendSoftContable.Models;

namespace BackendSoftContable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadEconomicaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActividadEconomicaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regimenes = await _context.ActividadEconomica
                .Select(r => new
                {
                    r.Id,
                    r.Descripcion
                })
                .ToListAsync();

            return Ok(regimenes);
        }

  
    }
}
