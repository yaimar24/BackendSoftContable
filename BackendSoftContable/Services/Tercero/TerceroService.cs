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
                    return new ApiResponseDTO<Guid> { Success = false, Message = "Tercero existente con esa misma categoria e identificación" };

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
                // 1️⃣ Cargar el tercero con responsabilidades e información fiscal
                var tercero = await _context.Tercero
                    .Include(t => t.Responsabilidades)
                    .Include(t => t.InformacionFiscal)
                    .FirstOrDefaultAsync(t => t.Id == dto.Id);

                if (tercero == null)
                    return ApiResponseDTO<Guid>.Fail("El tercero no existe.");

                // 2️⃣ Cargar la vinculación específica con Colegio y Categoría
                var vinculacion = await _context.TerceroCategoria
                    .FirstOrDefaultAsync(tc => tc.TerceroId == dto.Id && tc.ColegioId == dto.ColegioId && tc.CategoriaId == dto.CategoriaId);

                if (vinculacion == null)
                    return ApiResponseDTO<Guid>.Fail("No se encontró la vinculación con el colegio y categoría.");

                // 3️⃣ Verificar duplicados solo en otras filas
                var existeDuplicado = await _context.TerceroCategoria
                    .Include(tc => tc.Tercero)
                    .AnyAsync(tc =>
                        tc.ColegioId == dto.ColegioId &&
                        tc.CategoriaId == dto.CategoriaId &&
                        tc.Tercero.Identificacion == dto.Identificacion &&
                        tc.Id != vinculacion.Id && // Excluir la fila que se está actualizando
                        tc.Activo);

                if (existeDuplicado)
                    return ApiResponseDTO<Guid>.Fail("Ya existe un tercero con esa identificación en este colegio y categoría.");

                // 4️⃣ Actualizar datos globales del tercero
                tercero.TipoPersonaId = dto.TipoPersonaId;
                tercero.TipoIdentificacionId = dto.TipoIdentificacionId;
                tercero.Identificacion = dto.Identificacion;
                tercero.Dv = dto.Dv;
                tercero.Nombres = dto.Nombres;
                tercero.Apellidos = dto.Apellidos;
                tercero.NombreComercial = dto.NombreComercial;
                tercero.Email = dto.Email;
                tercero.Activo = dto.Activo;

                // 5️⃣ Actualizar información fiscal
                _mapper.Map(dto, tercero.InformacionFiscal);

                // 6️⃣ Actualizar datos de la vinculación (TerceroCategoria)
                vinculacion.RegimenIvaId = dto.RegimenIvaId;
                vinculacion.Direccion = dto.Direccion;
                vinculacion.Telefono = dto.Telefono;
                vinculacion.Activo = dto.Activo;

                // 7️⃣ Actualizar responsabilidades fiscales
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

                // 8️⃣ Guardar cambios
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponseDTO<Guid>.SuccessResponse(tercero.Id, "Tercero actualizado correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponseDTO<Guid>.Fail($"Error al actualizar: {ex.Message}");
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

                // Alternar el valor de Activo
                vinculacion.Activo = !vinculacion.Activo;

                // Opcional: actualizar info de modificación
                // vinculacion.UsuarioModificacionId = usuarioId;
                // vinculacion.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                string mensaje = vinculacion.Activo ?
                    "Tercero vinculado correctamente." :
                    "Tercero desvinculado correctamente.";

                return ApiResponseDTO<Guid>.SuccessResponse(terceroId, mensaje);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponseDTO<Guid>.Fail($"Error al actualizar la vinculación: {ex.Message}");
            }
        }

    }
}
