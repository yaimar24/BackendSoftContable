using BackendSoftContable.DTOs.Terceros;
using BackendSoftContable.DTOs;
using BackendSoftContable.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TerceroController : ControllerBase
{
    private readonly ITerceroService _service;

    public TerceroController(ITerceroService service)
    {
        _service = service;
    }

    private Guid GetUsuarioId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value!);
    private Guid GetColegioId() => Guid.Parse(User.FindFirst("colegioId")?.Value!);

    [HttpGet]
    public async Task<IActionResult> GetAllByColegio()
    {
        var response = await _service.ObtenerTodosPorColegio(GetColegioId());
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("vincular")]
    public async Task<IActionResult> Create([FromBody] TerceroCreateDTO dto)
    {
        var response = await _service.CreateWithCategoryAsync(dto, GetUsuarioId());
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TerceroEditDTO dto)
    {
        if (id != dto.Id)
            return BadRequest(ApiResponseDTO<Guid>.Fail("El Id de la URL no coincide con el cuerpo."));

        var response = await _service.UpdateAsync(dto, GetUsuarioId());
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPatch("status/{terceroId:guid}")]
    public async Task<IActionResult> DesvincularTercero(Guid terceroId)
    {
        var response = await _service.DesvincularTerceroAsync(terceroId, GetColegioId(), GetUsuarioId());
        return response.Success ? Ok(response) : BadRequest(response);
    }

    // 🔹 NUEVO ENDPOINT: Obtener clientes por colegio y opcionalmente filtrar por nombre
    [HttpGet("clientes")]
    public async Task<IActionResult> GetClientes([FromQuery] string? nombre)
    {
        var response = await _service.GetClientesAsync(GetColegioId(), nombre);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
