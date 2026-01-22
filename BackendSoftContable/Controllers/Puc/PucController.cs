using BackendSoftContable.DTOs.Puc;
using BackendSoftContable.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PucController : ControllerBase
{
    private readonly IPucService _pucService;

    public PucController(IPucService pucService)
    {
        _pucService = pucService;
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        var colegioId = Guid.Parse(User.FindFirst("colegioId")?.Value!);
        var result = await _pucService.GetTreeAsync(colegioId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PucCreateDTO dto)
    {
        var colegioId = Guid.Parse(User.FindFirst("colegioId")?.Value!);
        var usuarioId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var result = await _pucService.CreateAccountAsync(dto, colegioId, usuarioId);

        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}