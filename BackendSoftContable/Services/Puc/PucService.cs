using AutoMapper;
using BackendSoftContable.Data;
using BackendSoftContable.DTOs.Auditoria;
using BackendSoftContable.DTOs.Puc;
using BackendSoftContable.DTOs;
using BackendSoftContable.Interfaces.Services;
using BackendSoftContable.Models;
using BackendSoftContable.Utils;
using Microsoft.EntityFrameworkCore;

public class PucService : IPucService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;

    public PucService(AppDbContext context, IMapper mapper, IServiceScopeFactory scopeFactory)
    {
        _context = context;
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }

    public async Task<ApiResponseDTO<IEnumerable<PucNodoDTO>>> GetTreeAsync(Guid colegioId)
    {
        try
        {
            var listaPlana = await _context.Puc
                .Where(p => p.ColegioId == Guid.Empty || p.ColegioId == colegioId)
                .OrderBy(c => c.Codigo)
                .AsNoTracking() // 1. Evita que EF Core guarde estados previos en memoria
                .ToListAsync();

            // 2. Al crear el diccionario, forzamos que la lista de Hijos esté VACÍA
            var diccionario = new Dictionary<string, PucNodoDTO>();
            foreach (var c in listaPlana)
            {
                var dto = _mapper.Map<PucNodoDTO>(c);
                dto.Hijos = new List<PucNodoDTO>(); // <-- LIMPIEZA CLAVE
                diccionario[$"{c.Codigo}-{c.ColegioId}"] = dto;
            }

            var nodosRaiz = new List<PucNodoDTO>();
            foreach (var cuentaDb in listaPlana)
            {
                var llaveActual = $"{cuentaDb.Codigo}-{cuentaDb.ColegioId}";
                var nodoActual = diccionario[llaveActual];

                if (string.IsNullOrEmpty(cuentaDb.CodigoPadre))
                {
                    nodosRaiz.Add(nodoActual);
                }
                else
                {
                    var llavePadrePropio = $"{cuentaDb.CodigoPadre}-{cuentaDb.ColegioId}";
                    var llavePadreGlobal = $"{cuentaDb.CodigoPadre}-{Guid.Empty}";

                    if (diccionario.ContainsKey(llavePadrePropio))
                        diccionario[llavePadrePropio].Hijos.Add(nodoActual);
                    else if (diccionario.ContainsKey(llavePadreGlobal))
                        diccionario[llavePadreGlobal].Hijos.Add(nodoActual);
                }
            }
            return ApiResponseDTO<IEnumerable<PucNodoDTO>>.SuccessResponse(nodosRaiz);
        }
        catch (Exception ex)
        {
            return ApiResponseDTO<IEnumerable<PucNodoDTO>>.Fail(ex.Message);
        }
    }

    public async Task<ApiResponseDTO<string>> CreateAccountAsync(PucCreateDTO dto, Guid colegioId, Guid usuarioId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Validar existencia con Llave Compuesta
            var existe = await _context.Puc.AnyAsync(p =>
                p.Codigo == dto.Codigo && p.ColegioId == colegioId);

            if (existe) return ApiResponseDTO<string>.Fail("Esta cuenta ya existe en su colegio.");

            // 2. Validar Padre (Puede ser global o del colegio)
            Puc? padre = null;
            if (!string.IsNullOrEmpty(dto.CodigoPadre))
            {
                padre = await _context.Puc.FirstOrDefaultAsync(p =>
                    p.Codigo == dto.CodigoPadre && (p.ColegioId == Guid.Empty || p.ColegioId == colegioId));

                if (padre == null) return ApiResponseDTO<string>.Fail("La cuenta padre no existe.");
                if (padre.EsDetalle) return ApiResponseDTO<string>.Fail("No se puede crear subcuentas de una cuenta de detalle.");
            }

            // 3. Mapeo manual de campos calculados
            var nuevaCuenta = _mapper.Map<Puc>(dto);
            nuevaCuenta.ColegioId = colegioId; // Forzado desde el Token
            nuevaCuenta.Nivel = padre == null ? 1 : padre.Nivel + 1;
            nuevaCuenta.Naturaleza = padre != null ? padre.Naturaleza : dto.Naturaleza;
            nuevaCuenta.Nombre = dto.Nombre.ToUpper().Trim();
            nuevaCuenta.FechaCreacion = DateTime.Now;
            nuevaCuenta.Activo = true;

            _context.Puc.Add(nuevaCuenta);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // 4. Auditoría (Usamos el Código como referencia ya que no hay ID Guid)
            await AuditarAsync(
                AuditAccion.Crear,
                "Puc",
                $"Creación cuenta: {nuevaCuenta.Codigo}",
                null,
                new { nuevaCuenta.Codigo, nuevaCuenta.Nombre, nuevaCuenta.Nivel },
                usuarioId,
                colegioId);

            return ApiResponseDTO<string>.SuccessResponse(nuevaCuenta.Codigo, "Cuenta contable creada con éxito.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseDTO<string>.Fail($"Error en el servicio: {ex.Message}");
        }
    }

    private async Task AuditarAsync(AuditAccion accion, string entidad, string descripcion, object? antes, object? despues, Guid usuarioId, Guid colegioId)
    {
        var entry = new AuditEntry
        {
            Accion = accion.ToString().ToUpper(),
            Modulo = "Puc",
            Entidad = entidad,
            Descripcion = descripcion,
            DatosAntes = antes,
            DatosDespues = despues,
            Exitoso = true
        };
        var ctx = new AuditContext { UsuarioId = usuarioId, ColegioId = colegioId };

        using var scope = _scopeFactory.CreateScope();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
        await auditService.LogAsync(entry, ctx);
    }
}