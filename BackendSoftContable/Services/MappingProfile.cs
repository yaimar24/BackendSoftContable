using AutoMapper;
using BackendSoftContable.Models;
using BackendSoftContable.DTOs.Colegio;
using BackendSoftContable.DTOs.TerceroDetalleDTO;
using BackendSoftContable.Models.Terceros;

namespace BackendSoftContable.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Colegio básico y detalle
            CreateMap<Colegio, ColegioDTO>().ReverseMap();
            CreateMap<Colegio, ColegioDetailDTO>().ReverseMap();

            // ColegioCreateDTO -> Colegio
            CreateMap<ColegioCreateDTO, Colegio>()
                .ForMember(dest => dest.LogoPath, opt => opt.Ignore())
                // IMPORTANTE: Permitir que mapee la lista de representantes
                .ForMember(dest => dest.RepresentantesLegales, opt => opt.MapFrom(src => src.RepresentantesLegales));

            // ColegioUpdateDTO -> Colegio
            CreateMap<ColegioUpdateDTO, Colegio>()
                .ForMember(dest => dest.LogoPath, opt => opt.Ignore())
                .ForMember(dest => dest.RepresentantesLegales, opt => opt.Ignore());

            // RepresentanteLegal
            CreateMap<RepresentanteLegalDTO, RepresentanteLegal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // El ID lo genera la DB
                .ForMember(dest => dest.TipoIdentificacion, opt => opt.Ignore())
                .ForMember(dest => dest.Colegio, opt => opt.Ignore())
                .ReverseMap();

            // 1. DTO de creación -> Entidad Tercero (Datos Globales)
            // --- MAPEOS DE TERCEROS ---
            CreateMap<TerceroCreateDTO, Tercero>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TipoPersona, opt => opt.Ignore())
                .ForMember(dest => dest.TipoIdentificacion, opt => opt.Ignore())
                .ForMember(dest => dest.Categorias, opt => opt.Ignore());

            // 2. DTO de creación -> Entidad TerceroCategoria (Datos por Colegio)
            CreateMap<TerceroCreateDTO, TerceroCategoria>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TerceroId, opt => opt.Ignore()) // Se asigna en el servicio
                .ForMember(dest => dest.Tercero, opt => opt.Ignore())
                .ForMember(dest => dest.RegimenIva, opt => opt.Ignore());

            // 3. Entidad TerceroCategoria -> DTO de Detalle (Salida/Listados)
            CreateMap<TerceroCategoria, TerceroDetalleDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TerceroId))
                .ForMember(dest => dest.Identificacion, opt => opt.MapFrom(src => src.Tercero.Identificacion))
                .ForMember(dest => dest.Dv, opt => opt.MapFrom(src => src.Tercero.Dv))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Tercero.Email))
                .ForMember(dest => dest.TipoIdentificacionNombre, opt => opt.MapFrom(src => src.Tercero.TipoIdentificacion.Nombre))
                .ForMember(dest => dest.RegimenIvaNombre, opt => opt.MapFrom(src => src.RegimenIva.Nombre))
                // Lógica para Razón Social o Nombre Completo
                .ForMember(dest => dest.RazonSocial, opt => opt.MapFrom(src => 
                    !string.IsNullOrEmpty(src.Tercero.NombreComercial) 
                        ? src.Tercero.NombreComercial 
                        : $"{src.Tercero.Nombres} {src.Tercero.Apellidos}".Trim()));

            // 4. UpdateDTO -> TerceroCategoria
            CreateMap<TerceroDetalleDTO, TerceroCategoria>()
                .ForMember(dest => dest.TerceroId, opt => opt.Ignore())
                .ForMember(dest => dest.ColegioId, opt => opt.Ignore());

            // El DTO es la fuente única para 3 destinos
            CreateMap<TerceroCreateDTO, Tercero>();
            CreateMap<TerceroCreateDTO, TerceroInformacionFiscal>();
            CreateMap<TerceroCreateDTO, TerceroCategoria>();
        }
    }
}