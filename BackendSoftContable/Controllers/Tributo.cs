using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Data;
using BackendSoftContable.Models;

namespace BackendSoftContable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TributoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TributoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regimenes = await _context.Tributo
                .Select(r => new
                {
                    r.Id,
                    r.Nombre
                })
                .ToListAsync();

            return Ok(regimenes);
        }

  
    }
}
