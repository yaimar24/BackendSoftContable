using BackendSoftContable.DTOs.Colegio;
using BackendSoftContable.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BackendSoftContable.Services.Colegio;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class ColegioController : ControllerBase
{
    private readonly IColegioService _service;

    public ColegioController(IColegioService service)
    {
        _service = service;
    }

    // Registro de colegio + admin
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] ColegioCreateDTO dto)
    {
        var result = await _service.RegisterAsync(dto);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }
    [Authorize]
    [HttpGet("{id}/detalle")]
    public async Task<IActionResult> GetDetalle(Guid id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromForm] ColegioUpdateDTO dto)
    {
        // 1. Extraer ID del usuario del token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        // 2. Convertir a Guid de forma segura
        if (!Guid.TryParse(userIdClaim, out Guid usuarioId))
        {
            return BadRequest(new { Message = "El ID de usuario en el token no tiene un formato GUID válido." });
        }

        // 3. Llamar al servicio pasando el Guid
        var response = await _service.UpdateAsync(dto, usuarioId);

        return response.Success ? Ok(response) : BadRequest(response);
    }
}
