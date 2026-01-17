using AutoMapper;
using BackendSoftContable.Data;
using BackendSoftContable.DTOs;
using BackendSoftContable.Models.Terceros;
using BackendSoftContable.Models;
using BackendSoftContable.Repositories.ITerceroRepositories;
using BackendSoftContable.Repositories.ITercerosCategoria;
using Microsoft.EntityFrameworkCore;
using BackendSoftContable.DTOs.TerceroDetalleDTO;

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

        public async Task<ApiResponseDTO<IEnumerable<TerceroDetalleDTO>>> GetByColegioAsync(Guid colegioId)
        {
            // Usamos el repositorio para traer la data
            var data = await _categoriaRepo.GetByColegioAsync(colegioId);

            // Mapeamos a la lista de DTOs de detalle
            var result = _mapper.Map<IEnumerable<TerceroDetalleDTO>>(data);

            return new ApiResponseDTO<IEnumerable<TerceroDetalleDTO>>
            {
                Success = true,
                Data = result
            };
        }
    }
}