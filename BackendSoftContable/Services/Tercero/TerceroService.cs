using AutoMapper;
using BackendSoftContable.Data;
using BackendSoftContable.DTOs;
using BackendSoftContable.Models.Terceros;
using BackendSoftContable.Models;
using BackendSoftContable.Repositories.ITerceroRepositories;
using BackendSoftContable.Repositories.ITercerosCategoria;
using Microsoft.EntityFrameworkCore;
using BackendSoftContable.DTOs.Tercero;
using BackendSoftContable.DTOs.TerceroUpdateDTO;
using BackendSoftContable.DTOs.Terceros;

namespace BackendSoftContable.Services.TerceroService
{
    public class TerceroService : ITerceroService
    {
        private readonly AppDbContext _context;
        private readonly ITerceroRepository _terceroRepo;
        private readonly ITerceroCategoriaRepository _categoriaRepo;
        private readonly IMapper _mapper;

        public TerceroService(
            AppDbContext context,
            ITerceroRepository terceroRepo,
            ITerceroCategoriaRepository categoriaRepo,
            IMapper mapper)
        {
            _context = context;
            _terceroRepo = terceroRepo;
            _categoriaRepo = categoriaRepo;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<Guid>> CreateWithCategoryAsync(TerceroCreateDTO dto, Guid usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tercero = await _terceroRepo.GetByIdentificacionAsync(dto.Identificacion);
                bool esNuevoTercero = false;

                if (tercero == null)
                {
                    esNuevoTercero = true;
                    tercero = _mapper.Map<Tercero>(dto);
                    tercero.Id = Guid.NewGuid();
                    tercero.UsuarioCreacionId = usuarioId;
                    tercero.FechaRegistro = DateTime.Now;

                    var infoFiscal = _mapper.Map<TerceroInformacionFiscal>(dto);
                    infoFiscal.TerceroId = tercero.Id;
                    infoFiscal.UsuarioCreacionId = usuarioId;
                    infoFiscal.FechaRegistro = DateTime.Now;
                    tercero.InformacionFiscal = infoFiscal;


                    if (dto.ResponsabilidadesFiscalesIds?.Any() == true)
                    {
                        foreach (var respId in dto.ResponsabilidadesFiscalesIds)
                        {
                            tercero.Responsabilidades.Add(new TerceroResponsabilidad
                            {
                                TerceroId = tercero.Id,
                                ResponsabilidadFiscalId = respId,
                                UsuarioCreacionId = usuarioId,
                                FechaRegistro = DateTime.Now
                            });
                        }
                    }
                    await _context.Tercero.AddAsync(tercero);
                }

                var existeVinculo = await _categoriaRepo.ExistsAsync(tercero.Id, dto.ColegioId, dto.CategoriaId);
                if (existeVinculo)
                {
                    return new ApiResponseDTO<Guid> { Success = false, Message = "El tercero ya tiene esta categoría en el colegio." };
                }

                var vinculacion = _mapper.Map<TerceroCategoria>(dto);
                vinculacion.TerceroId = tercero.Id;
                vinculacion.UsuarioCreacionId = usuarioId;
                vinculacion.FechaRegistro = DateTime.Now;
                vinculacion.CiudadId = dto.CiudadId;

                await _context.TerceroCategoria.AddAsync(vinculacion);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponseDTO<Guid>
                {
                    Success = true,
                    Message = esNuevoTercero ? "Tercero creado y vinculado." : "Vínculo creado para tercero existente.",
                    Data = tercero.Id
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponseDTO<Guid> { Success = false, Message = $"Error: {ex.InnerException?.Message ?? ex.Message}" };
            }
        }
        public async Task<ApiResponseDTO<Guid>> UpdateAsync(TerceroEditDTO dto, Guid usuarioId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Cargar el Tercero con sus relaciones fiscales y de vinculación (TerceroCategoria)
                var tercero = await _context.Tercero
                    .Include(t => t.Responsabilidades)
                    .Include(t => t.InformacionFiscal)
                    .FirstOrDefaultAsync(t => t.Id == dto.Id);

                if (tercero == null)
                    return ApiResponseDTO<Guid>.Fail("El tercero no existe.");

                // 2. Cargar la vinculación específica con el colegio (donde vive la categoría, dirección y ciudad)
                var vinculacion = await _context.TerceroCategoria
                    .FirstOrDefaultAsync(tc => tc.TerceroId == dto.Id && tc.ColegioId == dto.ColegioId);

                if (vinculacion == null)
                    return ApiResponseDTO<Guid>.Fail("No se encontró la vinculación de este tercero con el colegio.");

                /* ==============================================
                   ACTUALIZACIÓN TABLA: TERCERO (Datos Globales)
                ================================================ */
                tercero.TipoPersonaId = dto.TipoPersonaId;
                tercero.TipoIdentificacionId = dto.TipoIdentificacionId;
                tercero.Identificacion = dto.Identificacion;
                tercero.Dv = dto.Dv;
                tercero.Nombres = dto.Nombres;
                tercero.Apellidos = dto.Apellidos;
                tercero.NombreComercial = dto.NombreComercial;
                tercero.Email = dto.Email;
                //tercero.UsuarioModificacionId = usuarioId;
                //tercero.FechaModificacion = DateTime.Now;

                /* ==============================================
                   ACTUALIZACIÓN TABLA: INFORMACION FISCAL
                ================================================ */
                if (tercero.InformacionFiscal != null)
                {
                    tercero.InformacionFiscal.Indicativo = dto.Indicativo;
                    tercero.InformacionFiscal.CodigoPostal = dto.CodigoPostal;
                    tercero.InformacionFiscal.ContactoNombres = dto.ContactoNombres;
                    tercero.InformacionFiscal.ContactoApellidos = dto.ContactoApellidos;
                    tercero.InformacionFiscal.CorreoFacturacion = dto.CorreoFacturacion;
                    //tercero.InformacionFiscal.UsuarioModificacionId = usuarioId;
                    //tercero.InformacionFiscal.FechaModificacion = DateTime.Now;
                }

                /* ==============================================
                   ACTUALIZACIÓN TABLA: TERCERO_CATEGORIA (Datos por Colegio)
                ================================================ */
                vinculacion.CategoriaId = dto.CategoriaId;
                vinculacion.RegimenIvaId = dto.RegimenIvaId;
                vinculacion.Direccion = dto.Direccion;
                vinculacion.Telefono = dto.Telefono;
                vinculacion.CiudadId = dto.CiudadId;
                //vinculacion.UsuarioModificacionId = usuarioId;
                //vinculacion.FechaModificacion = DateTime.Now;

                /* ==============================================
                   ACTUALIZACIÓN: RESPONSABILIDADES FISCALES
                ================================================ */
                // Limpiar actuales
                if (tercero.Responsabilidades?.Any() == true)
                {
                    _context.TerceroResponsabilidad.RemoveRange(tercero.Responsabilidades);
                }

                // Insertar nuevas
                if (dto.ResponsabilidadesFiscalesIds?.Any() == true)
                {
                    foreach (var respId in dto.ResponsabilidadesFiscalesIds)
                    {
                        await _context.TerceroResponsabilidad.AddAsync(new TerceroResponsabilidad
                        {
                            TerceroId = tercero.Id,
                            ResponsabilidadFiscalId = respId,
                            UsuarioCreacionId = usuarioId,
                            FechaRegistro = DateTime.Now
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponseDTO<Guid>.SuccessResponse(tercero.Id, "Tercero y vinculación actualizados correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponseDTO<Guid>.Fail(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<ApiResponseDTO<IEnumerable<TerceroEditDTO>>> ObtenerTodosPorColegio(Guid colegioId)
        {
            try
            {
                var terceros = await _context.TerceroCategoria
                    .Where(tc => tc.ColegioId == colegioId)
                    .Include(tc => tc.Tercero)
                        .ThenInclude(t => t.InformacionFiscal)
                    .Include(tc => tc.Tercero)
                        .ThenInclude(t => t.Responsabilidades)
                            .ThenInclude(tr => tr.ResponsabilidadFiscal)
                    .Include(tc => tc.Categoria)
                    .Include(tc => tc.RegimenIva)
                    .Select(tc => new TerceroEditDTO
                    {
                        Id = tc.TerceroId,
                        TipoPersonaId = tc.Tercero.TipoPersonaId,
                        TipoIdentificacionId = tc.Tercero.TipoIdentificacionId,
                        Identificacion = tc.Tercero.Identificacion,
                        Dv = tc.Tercero.Dv,
                        Nombres = tc.Tercero.Nombres,
                        Apellidos = tc.Tercero.Apellidos,
                        NombreComercial = tc.Tercero.NombreComercial,
                        Email = tc.Tercero.Email,
                        Telefono = tc.Telefono ?? "N/A",
                        Direccion = tc.Direccion,
                        CiudadId = tc.CiudadId,
                        Indicativo = tc.Tercero.InformacionFiscal.Indicativo,
                        CodigoPostal = tc.Tercero.InformacionFiscal.CodigoPostal,
                        ContactoNombres = tc.Tercero.InformacionFiscal.ContactoNombres,
                        ContactoApellidos = tc.Tercero.InformacionFiscal.ContactoApellidos,
                        CorreoFacturacion = tc.Tercero.InformacionFiscal.CorreoFacturacion,
                        ColegioId = tc.ColegioId,
                        CategoriaId = tc.CategoriaId,
                        RegimenIvaId = tc.RegimenIvaId,
                        ResponsabilidadesFiscalesIds = tc.Tercero.Responsabilidades
                            .Select(r => r.ResponsabilidadFiscalId)
                            .ToList()
                    })
                    .OrderBy(x => x.Nombres) // o RazonSocial si quieres normalizar en frontend
                    .ToListAsync();

                return ApiResponseDTO<IEnumerable<TerceroEditDTO>>
                    .SuccessResponse(terceros, "Terceros obtenidos con éxito.");
            }
            catch (Exception ex)
            {
                return ApiResponseDTO<IEnumerable<TerceroEditDTO>>
                    .Fail($"Error al obtener terceros: {ex.Message}");
            }
        }

    }    }