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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendSoftContable.Services.TerceroService
{
    public class TerceroService : ITerceroService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public TerceroService(AppDbContext context, IMapper mapper, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _mapper = mapper;
            _scopeFactory = scopeFactory;
        }

        public async Task<ApiResponseDTO<Guid>> CreateWithCategoryAsync(TerceroCreateDTO dto, Guid usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tercero = await _context.Tercero
                    .Include(t => t.InformacionFiscal)
                    .FirstOrDefaultAsync(t => t.Identificacion == dto.Identificacion);

                bool esNuevo = tercero == null;
                object? datosAntes = GetEstadoFiscal(tercero, esNuevo);

                if (esNuevo)
                {
                    tercero = _mapper.Map<Tercero>(dto);
                    tercero.Id = Guid.NewGuid();
                    tercero.UsuarioCreacionId = usuarioId;
                    tercero.FechaRegistro = DateTime.UtcNow;

                    tercero.InformacionFiscal = _mapper.Map<TerceroInformacionFiscal>(dto);
                    tercero.InformacionFiscal.TerceroId = tercero.Id;
                    tercero.InformacionFiscal.UsuarioCreacionId = usuarioId;
                    tercero.InformacionFiscal.FechaRegistro = DateTime.UtcNow;

                    tercero.Responsabilidades = MapearResponsabilidades(dto.ResponsabilidadesFiscalesIds, tercero.Id, usuarioId);

                    await _context.Tercero.AddAsync(tercero);
                }

                var vinculacion = _mapper.Map<TerceroCategoria>(dto);
                vinculacion.TerceroId = tercero.Id;
                vinculacion.UsuarioCreacionId = usuarioId;
                vinculacion.FechaRegistro = DateTime.UtcNow;
                vinculacion.Activo = true;

                await _context.TerceroCategoria.AddAsync(vinculacion);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Auditar(AuditAccion.Crear, "Tercero",
                    esNuevo ? "Creación y vinculación" : "Vinculación existente",
                    datosAntes, new { tercero.Id, dto.Identificacion }, usuarioId, dto.ColegioId);

                return ApiResponseDTO<Guid>.SuccessResponse(tercero.Id, "Procesado correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponseDTO<Guid>.Fail(ex.Message);
            }
        }

        public async Task<ApiResponseDTO<IEnumerable<TerceroEditDTO>>> ObtenerTodosPorColegio(Guid colegioId)
        {
            try
            {
                var lista = await _context.TerceroCategoria
                    .AsNoTracking() // Mejora de rendimiento para lectura
                    .Where(tc => tc.ColegioId == colegioId)
                    .Include(tc => tc.Tercero).ThenInclude(t => t.InformacionFiscal)
                    .Include(tc => tc.Tercero).ThenInclude(t => t.Responsabilidades)
                    .ToListAsync();

                var dtos = _mapper.Map<IEnumerable<TerceroEditDTO>>(lista);
                return ApiResponseDTO<IEnumerable<TerceroEditDTO>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponseDTO<IEnumerable<TerceroEditDTO>>.Fail(ex.Message);
            }
        }

        public async Task<ApiResponseDTO<Guid>> UpdateAsync(TerceroEditDTO dto, Guid usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tercero = await _context.Tercero
                    .Include(t => t.Responsabilidades)
                    .Include(t => t.InformacionFiscal)
                    .FirstOrDefaultAsync(t => t.Id == dto.Id);

                if (tercero == null) return ApiResponseDTO<Guid>.Fail("No existe.");

                var vinculacion = await _context.TerceroCategoria
                    .FirstOrDefaultAsync(tc => tc.TerceroId == dto.Id && tc.ColegioId == dto.ColegioId);

                var datosAntes = new
                {
                    tercero.Nombres,
                    vinculacion?.Direccion,
                    Responsabilidades = tercero.Responsabilidades.Select(r => r.ResponsabilidadFiscalId).ToList()
                };

                _mapper.Map(dto, tercero);
                if (tercero.InformacionFiscal != null) _mapper.Map(dto, tercero.InformacionFiscal);

                if (vinculacion != null)
                {
                    vinculacion.Direccion = dto.Direccion;
                    vinculacion.Telefono = dto.Telefono;
                    vinculacion.UsuarioActualizacionId = usuarioId;
                    vinculacion.FechaActualizacion = DateTime.UtcNow;
                }

                _context.TerceroResponsabilidad.RemoveRange(tercero.Responsabilidades);
                tercero.Responsabilidades = MapearResponsabilidades(dto.ResponsabilidadesFiscalesIds, tercero.Id, usuarioId);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Auditar(AuditAccion.Actualizar, "Tercero", $"Update {tercero.Identificacion}", datosAntes, dto, usuarioId, dto.ColegioId);

                return ApiResponseDTO<Guid>.SuccessResponse(tercero.Id, "Actualizado.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponseDTO<Guid>.Fail(ex.Message);
            }
        }

        public async Task<ApiResponseDTO<Guid>> DesvincularTerceroAsync(Guid terceroId, Guid colegioId, Guid usuarioId)
        {
            try
            {
                var vinculacion = await _context.TerceroCategoria
                    .FirstOrDefaultAsync(tc => tc.TerceroId == terceroId && tc.ColegioId == colegioId);

                if (vinculacion == null) return ApiResponseDTO<Guid>.Fail("No encontrado.");

                var antes = new { vinculacion.Activo };
                vinculacion.Activo = !vinculacion.Activo;
                vinculacion.UsuarioActualizacionId = usuarioId;
                vinculacion.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                Auditar(AuditAccion.CambioEstado, "TerceroCategoria",
                    $"Vínculo {(vinculacion.Activo ? "activado" : "desactivado")}",
                    antes, new { vinculacion.Activo }, usuarioId, colegioId);

                return ApiResponseDTO<Guid>.SuccessResponse(terceroId, "Estado cambiado.");
            }
            catch (Exception ex)
            {
                return ApiResponseDTO<Guid>.Fail(ex.Message);
            }
        }

        // --- MÉTODOS PRIVADOS ---

        private List<TerceroResponsabilidad> MapearResponsabilidades(List<int>? ids, Guid terceroId, Guid usuarioId)
        {
            return ids?.Select(id => new TerceroResponsabilidad
            {
                TerceroId = terceroId,
                ResponsabilidadFiscalId = id,
                UsuarioCreacionId = usuarioId,
                FechaRegistro = DateTime.UtcNow
            }).ToList() ?? new List<TerceroResponsabilidad>();
        }

        private object? GetEstadoFiscal(Tercero? t, bool esNuevo) => !esNuevo ? new
        {
            t!.Id,
            t.Nombres,
            Fiscal = t.InformacionFiscal != null ? new { t.InformacionFiscal.CorreoFacturacion } : null
        } : null;

        private void Auditar(AuditAccion accion, string entidad, string desc, object? antes, object? despues, Guid user, Guid col)
        {
            // Capturamos los datos en variables locales fuera del Task.Run
            // para evitar que el context disposed rompa la ejecución
            var entry = new AuditEntry
            {
                Accion = accion.ToString().ToUpper(),
                Modulo = "Terceros",
                Entidad = entidad,
                Descripcion = desc,
                DatosAntes = antes,
                DatosDespues = despues,
                Exitoso = true
            };

            var ctx = new AuditContext
            {
                UsuarioId = user,
                ColegioId = col,
            };

            _ = Task.Run(async () =>
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                        await auditService.LogAsync(entry, ctx);
                    }
                }
                catch (Exception ex)
                {
         
                }
            });
        }
    }
}