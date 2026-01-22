using AutoMapper;
using BackendSoftContable.Data;
using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Auditoria;
using BackendSoftContable.DTOs.Tercero;
using BackendSoftContable.DTOs.Terceros;
using BackendSoftContable.Interfaces.Services;
using BackendSoftContable.Models;
using BackendSoftContable.Models.Terceros;
using BackendSoftContable.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace BackendSoftContable.Services.TerceroService;

public class TerceroService : ITerceroService
{
    private readonly ITerceroRepository _terceroRepo;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;

    public TerceroService(
        ITerceroRepository terceroRepo,
        AppDbContext context,
        IMapper mapper,
        IServiceScopeFactory scopeFactory)
    {
        _terceroRepo = terceroRepo;
        _context = context;
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }

    // ========================= CREATE =========================

    public async Task<ApiResponseDTO<Guid>> CreateWithCategoryAsync(
        TerceroCreateDTO dto,
        Guid usuarioId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var tercero = await _terceroRepo
                .GetByIdentificacionCompletoAsync(dto.Identificacion);

            bool esNuevo = tercero == null;
            object? datosAntes = GetEstadoFiscal(tercero, esNuevo);

            if (esNuevo)
            {
                tercero = _mapper.Map<Tercero>(dto);
                tercero.Id = Guid.NewGuid();
                tercero.UsuarioCreacionId = usuarioId;
                tercero.FechaRegistro = DateTime.Now;

                tercero.InformacionFiscal =
                    _mapper.Map<TerceroInformacionFiscal>(dto);

                tercero.InformacionFiscal.TerceroId = tercero.Id;
                tercero.InformacionFiscal.UsuarioCreacionId = usuarioId;
                tercero.InformacionFiscal.FechaRegistro = DateTime.Now;

                tercero.Responsabilidades =
                    MapearResponsabilidades(dto.ResponsabilidadesFiscalesIds,
                        tercero.Id, usuarioId);

                await _terceroRepo.AddAsync(tercero);
            }

            var vinculacionExistente =
                await _terceroRepo.GetVinculacionAsync(tercero!.Id, dto.ColegioId);

            if (vinculacionExistente != null)
                return ApiResponseDTO<Guid>
                    .Fail("El tercero ya se encuentra vinculado a este colegio.");

            var vinculacion = _mapper.Map<TerceroCategoria>(dto);
            vinculacion.TerceroId = tercero.Id;
            vinculacion.UsuarioCreacionId = usuarioId;
            vinculacion.FechaRegistro = DateTime.UtcNow;
            vinculacion.Activo = true;

            await _terceroRepo.AddVinculacionAsync(vinculacion);
            await _terceroRepo.SaveChangesAsync();

            await transaction.CommitAsync();

            await AuditarAsync(
                AuditAccion.Crear,
                "Tercero",
                esNuevo
                    ? "Registro completo y vinculación"
                    : "Vinculación de tercero existente",
                datosAntes,
                new { tercero.Id, dto.Identificacion },
                usuarioId,
                dto.ColegioId);

            return ApiResponseDTO<Guid>
                .SuccessResponse(tercero.Id, "Procesado correctamente.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseDTO<Guid>
                .Fail($"Error al crear tercero: {ex.Message}");
        }
    }

    // ========================= UPDATE =========================

    public async Task<ApiResponseDTO<Guid>> UpdateAsync(
        TerceroEditDTO dto,
        Guid usuarioId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var tercero = await _terceroRepo.GetByIdCompletoAsync(dto.Id);
            if (tercero == null)
                return ApiResponseDTO<Guid>.Fail("Tercero no encontrado.");

            var vinculacion =
                await _terceroRepo.GetVinculacionAsync(dto.Id, dto.ColegioId);

            var datosAntes = new
            {
                tercero.Nombres,
                tercero.Identificacion,
                vinculacion?.Direccion,
                Responsabilidades = tercero.Responsabilidades
                    .Select(r => r.ResponsabilidadFiscalId)
                    .ToList()
            };

            _mapper.Map(dto, tercero);

            if (tercero.InformacionFiscal != null)
                _mapper.Map(dto, tercero.InformacionFiscal);

            if (vinculacion != null)
            {
                vinculacion.Direccion = dto.Direccion;
                vinculacion.Telefono = dto.Telefono;
                vinculacion.UsuarioActualizacionId = usuarioId;
                vinculacion.FechaActualizacion = DateTime.Now;
            }

            _terceroRepo.RemoveResponsabilidades(tercero.Responsabilidades);
            tercero.Responsabilidades =
                MapearResponsabilidades(dto.ResponsabilidadesFiscalesIds,
                    tercero.Id, usuarioId);

            await _terceroRepo.SaveChangesAsync();
            await transaction.CommitAsync();

            await AuditarAsync(
                AuditAccion.Actualizar,
                "Tercero",
                $"Actualización de datos: {tercero.Identificacion}",
                datosAntes,
                dto,
                usuarioId,
                dto.ColegioId);

            return ApiResponseDTO<Guid>
                .SuccessResponse(tercero.Id, "Actualizado correctamente.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponseDTO<Guid>
                .Fail($"Error al actualizar: {ex.Message}");
        }
    }

    // ========================= GET =========================

    public async Task<ApiResponseDTO<IEnumerable<TerceroEditDTO>>>
        ObtenerTodosPorColegio(Guid colegioId)
    {
        try
        {
            var lista =
                await _terceroRepo.GetTodosPorColegioAsync(colegioId);

            var dtos =
                _mapper.Map<IEnumerable<TerceroEditDTO>>(lista);

            return ApiResponseDTO<IEnumerable<TerceroEditDTO>>
                .SuccessResponse(dtos);
        }
        catch (Exception ex)
        {
            return ApiResponseDTO<IEnumerable<TerceroEditDTO>>
                .Fail(ex.Message);
        }
    }

    // ========================= DESVINCULAR =========================

    public async Task<ApiResponseDTO<Guid>> DesvincularTerceroAsync(
        Guid terceroId,
        Guid colegioId,
        Guid usuarioId)
    {
        try
        {
            var vinculacion =
                await _terceroRepo.GetVinculacionAsync(terceroId, colegioId);

            if (vinculacion == null)
                return ApiResponseDTO<Guid>.Fail("Vinculación no encontrada.");

            var antes = new { vinculacion.Activo };

            vinculacion.Activo = !vinculacion.Activo;
            vinculacion.UsuarioActualizacionId = usuarioId;
            vinculacion.FechaActualizacion = DateTime.Now;

            await _terceroRepo.SaveChangesAsync();

            await AuditarAsync(
                AuditAccion.CambioEstado,
                "TerceroCategoria",
                $"Vínculo {(vinculacion.Activo ? "activado" : "desactivado")}",
                antes,
                new { vinculacion.Activo },
                usuarioId,
                colegioId);

            return ApiResponseDTO<Guid>
                .SuccessResponse(terceroId, "Estado actualizado.");
        }
        catch (Exception ex)
        {
            return ApiResponseDTO<Guid>.Fail(ex.Message);
        }
    }

    // ========================= HELPERS =========================

    private List<TerceroResponsabilidad> MapearResponsabilidades(
        List<int>? ids,
        Guid terceroId,
        Guid usuarioId)
    {
        return ids?.Select(id => new TerceroResponsabilidad
        {
            TerceroId = terceroId,
            ResponsabilidadFiscalId = id,
            UsuarioCreacionId = usuarioId,
            FechaRegistro = DateTime.Now
        }).ToList() ?? new();
    }

    private object? GetEstadoFiscal(Tercero? t, bool esNuevo) =>
        !esNuevo
            ? new { t!.Id, t.Nombres, t.Identificacion }
            : null;


    private async Task AuditarAsync(
        AuditAccion accion,
        string entidad,
        string descripcion,
        object? antes,
        object? despues,
        Guid usuarioId,
        Guid colegioId)
    {
        var entry = new AuditEntry
        {
            Accion = accion.ToString().ToUpper(),
            Modulo = "Terceros",
            Entidad = entidad,
            Descripcion = descripcion,
            DatosAntes = antes,
            DatosDespues = despues,
            Exitoso = true
        };

        var ctx = new AuditContext
        {
            UsuarioId = usuarioId,
            ColegioId = colegioId
        };

        using var scope = _scopeFactory.CreateScope();
        var auditService =
            scope.ServiceProvider.GetRequiredService<IAuditService>();

        await auditService.LogAsync(entry, ctx);
    }
}
