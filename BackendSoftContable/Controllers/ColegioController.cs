using BackendSoftContable.DTOs.Colegio;
using BackendSoftContable.DTOs;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> Get(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("{id}/detalle")]
    public async Task<IActionResult> GetDetalle(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] ColegioUpdateDTO dto)
    {
        if (id != dto.Id) return BadRequest("El ID no coincide");

        var result = await _service.UpdateAsync(dto);

        if (!result.Success) return NotFound(result);

        return Ok(result);
    }
}
