using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Data;
using BackendSoftContable.Models;

namespace BackendSoftContable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CiudadController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CiudadController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regimenes = await _context.Ciudad
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
