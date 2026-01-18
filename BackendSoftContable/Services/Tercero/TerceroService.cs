using AutoMapper;
using BackendSoftContable.Data;
using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Tercero;
using BackendSoftContable.DTOs.Terceros;
using BackendSoftContable.Models;
using BackendSoftContable.Models.Terceros;
using Microsoft.EntityFrameworkCore;

namespace BackendSoftContable.Services.TerceroService
{
    public class TerceroService : ITerceroService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TerceroService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // -------------------------------
        // Crear y vincular tercero
        // -------------------------------
        public async Task<ApiResponseDTO<Guid>> CreateWithCategoryAsync(TerceroCreateDTO dto, Guid usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Buscar tercero existente
                var tercero = await _context.Tercero
                    .Include(t => t.InformacionFiscal)
                    .Include(t => t.Responsabilidades)
                    .FirstOrDefaultAsync(t => t.Identificacion == dto.Identificacion);

                bool esNuevo = tercero == null;

                if (esNuevo)
                {
                    tercero = _mapper.Map<Tercero>(dto);
                    tercero.Id = Guid.NewGuid();
                    tercero.UsuarioCreacionId = usuarioId;
                    tercero.FechaRegistro = DateTime.Now;

                    // Información fiscal
                    tercero.InformacionFiscal = _mapper.Map<TerceroInformacionFiscal>(dto);
                    tercero.InformacionFiscal.TerceroId = tercero.Id;
                    tercero.InformacionFiscal.UsuarioCreacionId = usuarioId;
                    tercero.InformacionFiscal.FechaRegistro = DateTime.Now;

                    // Responsabilidades fiscales
                    tercero.Responsabilidades = dto.ResponsabilidadesFiscalesIds?
                        .Select(id => new TerceroResponsabilidad
                        {
                            TerceroId = tercero.Id,
                            ResponsabilidadFiscalId = id,
                            UsuarioCreacionId = usuarioId,
                            FechaRegistro = DateTime.Now
                        }).ToList() ?? new List<TerceroResponsabilidad>();

                    await _context.Tercero.AddAsync(tercero);
                }

                // Vinculación con colegio/categoría
                var existeVinculo = await _context.TerceroCategoria
                    .AnyAsync(tc => tc.TerceroId == tercero.Id && tc.ColegioId == dto.ColegioId && tc.CategoriaId == dto.CategoriaId);

                if (existeVinculo)
                    return new ApiResponseDTO<Guid> { Success = false, Message = "El tercero ya tiene esta categoría en el colegio." };

                var vinculacion = _mapper.Map<TerceroCategoria>(dto);
                vinculacion.TerceroId = tercero.Id;
                vinculacion.UsuarioCreacionId = usuarioId;
                vinculacion.FechaRegistro = DateTime.Now;

                await _context.TerceroCategoria.AddAsync(vinculacion);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponseDTO<Guid>
                {
                    Success = true,
                    Message = esNuevo ? "Tercero creado y vinculado." : "Vínculo creado para tercero existente.",
                    Data = tercero.Id
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponseDTO<Guid> { Success = false, Message = ex.Message };
            }
        }

        // -------------------------------
        // Actualizar tercero
        // -------------------------------
        public async Task<ApiResponseDTO<Guid>> UpdateAsync(TerceroEditDTO dto, Guid usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var tercero = await _context.Tercero
                    .Include(t => t.Responsabilidades)
                    .Include(t => t.InformacionFiscal)
                    .FirstOrDefaultAsync(t => t.Id == dto.Id);

                if (tercero == null)
                    return ApiResponseDTO<Guid>.Fail("El tercero no existe.");

                // Vinculación específica
                var vinculacion = await _context.TerceroCategoria
                    .FirstOrDefaultAsync(tc => tc.TerceroId == dto.Id && tc.ColegioId == dto.ColegioId);

                if (vinculacion == null)
                    return ApiResponseDTO<Guid>.Fail("No se encontró la vinculación con el colegio.");

                // Mapear cambios usando AutoMapper
                _mapper.Map(dto, tercero);          // Datos globales
                _mapper.Map(dto, tercero.InformacionFiscal); // Datos fiscales
                _mapper.Map(dto, vinculacion);      // Datos por colegio

                // Actualizar responsabilidades
                if (tercero.Responsabilidades?.Any() == true)
                    _context.TerceroResponsabilidad.RemoveRange(tercero.Responsabilidades);

                if (dto.ResponsabilidadesFiscalesIds?.Any() == true)
                {
                    tercero.Responsabilidades = dto.ResponsabilidadesFiscalesIds
                        .Select(id => new TerceroResponsabilidad
                        {
                            TerceroId = tercero.Id,
                            ResponsabilidadFiscalId = id,
                            UsuarioCreacionId = usuarioId,
                            FechaRegistro = DateTime.Now
                        }).ToList();
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponseDTO<Guid>.SuccessResponse(tercero.Id, "Tercero actualizado correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponseDTO<Guid>.Fail(ex.Message);
            }
        }

        // -------------------------------
        // Obtener todos los terceros por colegio
        // -------------------------------
        public async Task<ApiResponseDTO<IEnumerable<TerceroEditDTO>>> ObtenerTodosPorColegio(Guid colegioId)
        {
            try
            {
                var tercerosCategorias = await _context.TerceroCategoria
                    .Where(tc => tc.ColegioId == colegioId)
                    .Include(tc => tc.Tercero)
                        .ThenInclude(t => t.InformacionFiscal)
                    .Include(tc => tc.Tercero)
                        .ThenInclude(t => t.Responsabilidades)
                    .ToListAsync();

                var result = _mapper.Map<IEnumerable<TerceroEditDTO>>(tercerosCategorias);

                return ApiResponseDTO<IEnumerable<TerceroEditDTO>>.SuccessResponse(result, "Terceros obtenidos con éxito.");
            }
            catch (Exception ex)
            {
                return ApiResponseDTO<IEnumerable<TerceroEditDTO>>.Fail($"Error al obtener terceros: {ex.Message}");
            }
        }

        // -------------------------------
        // Desvincular tercero
        // -------------------------------
        public async Task<ApiResponseDTO<Guid>> DesvincularTerceroAsync(Guid terceroId, Guid colegioId, Guid usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Buscar la vinculación específica
                var vinculacion = await _context.TerceroCategoria
                    .FirstOrDefaultAsync(tc => tc.TerceroId == terceroId && tc.ColegioId == colegioId);

                if (vinculacion == null)
                    return ApiResponseDTO<Guid>.Fail("No se encontró la vinculación con este colegio.");

                // Marcar como inactivo
                vinculacion.Activo = false;
                //vinculacion.UsuarioModificacionId = usuarioId;
                //vinculacion.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponseDTO<Guid>.SuccessResponse(terceroId, "Tercero desvinculado correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponseDTO<Guid>.Fail($"Error al desvincular el tercero: {ex.Message}");
            }
        }

    }
}
