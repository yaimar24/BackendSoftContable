using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendSoftContable.Data;
using BackendSoftContable.DTOs.Puc;

namespace BackendSoftContable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PucController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PucController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("tree")]
        public async Task<IActionResult> GetTree()
        {
            // 1. Traemos todos los datos planos (o solo los activos)
            var todasLasCuentas = await _context.Puc
            .OrderBy(c => c.Codigo)
                .Select(c => new PucNodoDTO
                {
                    Codigo = c.Codigo,
                    Nombre = c.Nombre,
                    Nivel = c.Nivel,
                    EsDetalle = c.EsDetalle,
                    Naturaleza = c.Naturaleza,
                    // El CodigoPadre lo necesitamos temporalmente para armar el árbol
                    // pero no es necesario enviarlo al front si el objeto ya está anidado
                })
                .ToListAsync();

            // 2. Traemos también la relación plana para poder armar el árbol en memoria
            var listaPlana = await _context.Puc.ToListAsync();

            // 3. Diccionario para búsqueda rápida y armado del árbol
            var diccionario = todasLasCuentas.ToDictionary(c => c.Codigo);
            var nodosRaiz = new List<PucNodoDTO>();

            foreach (var cuentaDb in listaPlana)
            {
                var nodoActual = diccionario[cuentaDb.Codigo];

                if (string.IsNullOrEmpty(cuentaDb.CodigoPadre))
                {
                    // Es una Clase (Nivel 1), va a la raíz
                    nodosRaiz.Add(nodoActual);
                }
                else if (diccionario.ContainsKey(cuentaDb.CodigoPadre))
                {
                    // Buscamos al padre en el diccionario y le añadimos este hijo
                    diccionario[cuentaDb.CodigoPadre].Hijos.Add(nodoActual);
                }
            }

            return Ok(nodosRaiz);
        }
    }
}