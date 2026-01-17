using BackendSoftContable.DTOs.Terceros;
using BackendSoftContable.DTOs.TerceroUpdateDTO;
using BackendSoftContable.Services.TerceroService;
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
        [HttpGet]
        public async Task<IActionResult> GetAllByColegio()
        {
            // Extraemos el colegioId del claim del JWT
            var colegioIdClaim = User.FindFirst("colegioId")?.Value;

            if (string.IsNullOrEmpty(colegioIdClaim))
                return Unauthorized(new { message = "El token no contiene la información del colegio" });

            var colegioId = Guid.Parse(colegioIdClaim);

            // Llamamos al servicio que trae todos los terceros de ese colegio
            var response = await _service.ObtenerTodosPorColegio(colegioId);

            return response.Success ? Ok(response) : NotFound(response);
        }



        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TerceroEditDTO dto)
        {
            // 1. Validación del modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Datos del formulario inválidos",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            // 2. Validar coherencia entre URL y body
            if (id != dto.Id)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "El Id de la URL no coincide con el del cuerpo"
                });
            }

            // 3. Obtener usuarioId desde el JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid usuarioId))
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Token de usuario inválido o expirado"
                });
            }

            // 4. Llamar al servicio
            var response = await _service.UpdateAsync(dto, usuarioId);

            // 5. Retornar respuesta estándar
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
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