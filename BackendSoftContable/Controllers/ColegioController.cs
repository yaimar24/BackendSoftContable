using Microsoft.AspNetCore.Mvc;
using BackendSoftContable.DTOs;
using BackendSoftContable.Services;

namespace BackendSoftContable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColegioController : ControllerBase
    {
        private readonly IColegioService _service;

        public ColegioController(IColegioService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ColegioCreateDTO dto)
        {
            try
            {
                var result = await _service.RegisterColegioAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _service.GetColegioByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
    }
}
