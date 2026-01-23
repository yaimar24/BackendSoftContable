using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Models.TipoFactura;
using BackendSoftContable.Data;
using BackendSoftContable.DTOs;
using Microsoft.AspNetCore.Authorization;
// Asegúrate de importar el namespace de tu DataContext aquí
// using BackendSoftContable.Data; 

namespace BackendSoftContable.Controllers.TipoFactura
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoFacturaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TipoFacturaController(AppDbContext context)
        {
            _context = context;
        }

        // Fix for the CS1503 error in the GetTiposFactura method
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BackendSoftContable.Models.TipoFactura.TipoFactura>>> GetTiposFactura()
        {
            var tiposFactura = await _context.TipoFacturas.ToListAsync();
            return Ok(ApiResponseDTO<IEnumerable<BackendSoftContable.Models.TipoFactura.TipoFactura>>.SuccessResponse(tiposFactura));
        }

        // Fix for the errors in the GetTipoFactura method
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDTO<BackendSoftContable.Models.TipoFactura.TipoFactura>>> GetTipoFactura(int id)
        {
            var tipoFactura = await _context.TipoFacturas.FindAsync(id);

            if (tipoFactura == null)
                return NotFound(ApiResponseDTO<BackendSoftContable.Models.TipoFactura.TipoFactura>.Fail("TipoFactura not found"));

            return Ok(ApiResponseDTO<BackendSoftContable.Models.TipoFactura.TipoFactura>.SuccessResponse(tipoFactura));
        }

        // POST: api/TipoFactura
        [HttpPost]
        public async Task<ActionResult<BackendSoftContable.Models.TipoFactura.TipoFactura>> PostTipoFactura(BackendSoftContable.Models.TipoFactura.TipoFactura tipoFactura)
        {
            _context.TipoFacturas.Add(tipoFactura);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTipoFactura), new { id = tipoFactura.Id }, tipoFactura);
        }

        // PUT: api/TipoFactura/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoFactura(int id, BackendSoftContable.Models.TipoFactura.TipoFactura tipoFactura)
        {
            if (id != tipoFactura.Id) return BadRequest();

            _context.Entry(tipoFactura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoFacturaExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/TipoFactura/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoFactura(int id)
        {
            var tipoFactura = await _context.TipoFacturas.FindAsync(id);
            if (tipoFactura == null) return NotFound();

            _context.TipoFacturas.Remove(tipoFactura);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Eliminado correctamente" });
        }

        private bool TipoFacturaExists(int id)
        {
            return _context.TipoFacturas.Any(e => e.Id == id);
        }
    }
}