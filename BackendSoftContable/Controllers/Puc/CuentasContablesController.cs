using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BackendSoftContable.Data;
using BackendSoftContable.Models;
using BackendSoftContable.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BackendSoftContable.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasContablesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CuentasContablesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PucCreateDto dto)
        {
            // --- 1. EXTRAER EL USUARIO DEL TOKEN (Tu lógica ejemplo) ---
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            if (!Guid.TryParse(userIdClaim, out Guid usuarioId))
            {
                return BadRequest(new { Message = "ID de usuario inválido en el token." });
            }

            // --- 2. VALIDACIONES DE NEGOCIO ---
            var existe = await _context.CuentasContables.AnyAsync(c => c.Codigo == dto.Codigo);
            if (existe) return BadRequest("Esta cuenta ya existe en su plan contable.");

            // --- 3. CÁLCULOS AUTOMÁTICOS (Nivel y Naturaleza) ---
            int nivelCalculado = 1;
            string naturalezaCalculada = dto.Naturaleza ?? "D";

            if (!string.IsNullOrEmpty(dto.CodigoPadre))
            {
                var padre = await _context.CuentasContables.FirstOrDefaultAsync(p => p.Codigo == dto.CodigoPadre);
                if (padre == null) return BadRequest("La cuenta padre no existe en su plan personalizado.");

                nivelCalculado = padre.Nivel + 1;
                naturalezaCalculada = padre.Naturaleza; // Hereda del padre siempre

                if (padre.EsDetalle)
                {
                    padre.EsDetalle = false; // El padre ahora es de control
                    padre.FechaActualizacion = DateTime.Now;
                    padre.UsuarioActualizacionId = usuarioId;
                }
            }

            // --- 4. CREAR ENTIDAD CON AUDITORÍA (BaseEntity) ---
            var nuevaCuenta = new CuentaContable
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre.ToUpper(),
                CodigoPadre = dto.CodigoPadre,
                Nivel = nivelCalculado,
                Naturaleza = naturalezaCalculada,
                EsDetalle = true,
                Activa = true,

                // Campos de BaseEntity
                FechaRegistro = DateTime.Now,
                UsuarioCreacionId = usuarioId // <--- Aquí usamos el Guid extraído
            };

            _context.CuentasContables.Add(nuevaCuenta);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Data = nuevaCuenta });
        }
    }
}