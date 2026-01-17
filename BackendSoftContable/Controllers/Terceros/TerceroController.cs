using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendSoftContable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerceroController : ControllerBase
    {
        private readonly ITerceroService _service;

        public TerceroController(ITerceroService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost("vincular")]
        public async Task<IActionResult> Create([FromBody] TerceroCreateDTO dto)
        {
            // 1. Validación de Modelo (Asegura que los DataAnnotations del DTO se cumplan)
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Datos del formulario inválidos", Errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            // 2. Extracción segura del UsuarioId del Token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid usuarioId))
            {
                return Unauthorized(new { Success = false, Message = "Token de usuario inválido o expirado" });
            }

            // 3. Llamada al servicio (El servicio se encarga de la lógica de negocio y transacción)
            var response = await _service.CreateWithCategoryAsync(dto, usuarioId);

            // 4. Retorno de respuesta estandarizada
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}