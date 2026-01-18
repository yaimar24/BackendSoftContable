using AutoMapper;
using BackendSoftContable.Models;
using BackendSoftContable.DTOs.Colegio;
using BackendSoftContable.Models.Terceros;
using BackendSoftContable.DTOs.Tercero;
using BackendSoftContable.DTOs.Terceros;

namespace BackendSoftContable.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // -------------------------------
            // Colegio
            // -------------------------------
            CreateMap<Colegio, ColegioDTO>().ReverseMap();
            CreateMap<Colegio, ColegioDetailDTO>().ReverseMap();

            CreateMap<ColegioCreateDTO, Colegio>()
                .ForMember(dest => dest.LogoPath, opt => opt.Ignore())
                .ForMember(dest => dest.RepresentantesLegales, opt => opt.MapFrom(src => src.RepresentantesLegales));

            CreateMap<ColegioUpdateDTO, Colegio>()
                .ForMember(dest => dest.LogoPath, opt => opt.Ignore())
                .ForMember(dest => dest.RepresentantesLegales, opt => opt.Ignore());

            // Representante legal
            CreateMap<RepresentanteLegalDTO, RepresentanteLegal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TipoIdentificacion, opt => opt.Ignore())
                .ForMember(dest => dest.Colegio, opt => opt.Ignore())
                .ReverseMap();

            // -------------------------------
            // Tercero
            // -------------------------------

            // 1. DTO de creación -> Entidad Tercero (Datos Globales)
            CreateMap<TerceroCreateDTO, Tercero>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TipoPersona, opt => opt.Ignore())
                .ForMember(dest => dest.TipoIdentificacion, opt => opt.Ignore())
                .ForMember(dest => dest.Categorias, opt => opt.Ignore());

            // 2. DTO de creación -> Entidad TerceroCategoria (Datos por Colegio)
            CreateMap<TerceroCreateDTO, TerceroCategoria>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TerceroId, opt => opt.Ignore())
                .ForMember(dest => dest.Tercero, opt => opt.Ignore())
                .ForMember(dest => dest.RegimenIva, opt => opt.Ignore());

            // 3. DTO de creación -> TerceroInformacionFiscal
            CreateMap<TerceroCreateDTO, TerceroInformacionFiscal>()
                .ForMember(dest => dest.TerceroId, opt => opt.Ignore());

            // 4. Entidad TerceroCategoria -> DTO de Detalle (para listados)
            CreateMap<TerceroCategoria, TerceroEditDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TerceroId))
                .ForMember(dest => dest.TipoPersonaId, opt => opt.MapFrom(src => src.Tercero.TipoPersonaId))
                .ForMember(dest => dest.TipoIdentificacionId, opt => opt.MapFrom(src => src.Tercero.TipoIdentificacionId))
                .ForMember(dest => dest.Identificacion, opt => opt.MapFrom(src => src.Tercero.Identificacion))
                .ForMember(dest => dest.Dv, opt => opt.MapFrom(src => src.Tercero.Dv))
                .ForMember(dest => dest.Nombres, opt => opt.MapFrom(src => src.Tercero.Nombres))
                .ForMember(dest => dest.Apellidos, opt => opt.MapFrom(src => src.Tercero.Apellidos))
                .ForMember(dest => dest.NombreComercial, opt => opt.MapFrom(src => src.Tercero.NombreComercial))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Tercero.Email))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.Telefono))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.CiudadId, opt => opt.MapFrom(src => src.CiudadId))
                .ForMember(dest => dest.Indicativo, opt => opt.MapFrom(src => src.Tercero.InformacionFiscal.Indicativo))
                .ForMember(dest => dest.CodigoPostal, opt => opt.MapFrom(src => src.Tercero.InformacionFiscal.CodigoPostal))
                .ForMember(dest => dest.ContactoNombres, opt => opt.MapFrom(src => src.Tercero.InformacionFiscal.ContactoNombres))
                .ForMember(dest => dest.ContactoApellidos, opt => opt.MapFrom(src => src.Tercero.InformacionFiscal.ContactoApellidos))
                .ForMember(dest => dest.CorreoFacturacion, opt => opt.MapFrom(src => src.Tercero.InformacionFiscal.CorreoFacturacion))
                .ForMember(dest => dest.ColegioId, opt => opt.MapFrom(src => src.ColegioId))
                .ForMember(dest => dest.CategoriaId, opt => opt.MapFrom(src => src.CategoriaId))
                .ForMember(dest => dest.RegimenIvaId, opt => opt.MapFrom(src => src.RegimenIvaId))
                .ForMember(dest => dest.ResponsabilidadesFiscalesIds, opt => opt.MapFrom(src => src.Tercero.Responsabilidades.Select(r => r.ResponsabilidadFiscalId)));

            // 5. DTO de detalle -> TerceroCategoria (para updates)
            CreateMap<TerceroEditDTO, TerceroCategoria>()
                .ForMember(dest => dest.TerceroId, opt => opt.Ignore())
                .ForMember(dest => dest.ColegioId, opt => opt.Ignore());


            // Mapear TerceroEditDTO -> Tercero (actualización de datos globales)
            CreateMap<TerceroEditDTO, Tercero>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // usualmente se ignora si no quieres cambiar el Id
                .ForMember(dest => dest.Categorias, opt => opt.Ignore()) // si Tercero tiene colección de categorías
                .ForMember(dest => dest.TipoPersona, opt => opt.Ignore())
                .ForMember(dest => dest.TipoIdentificacion, opt => opt.Ignore());

            CreateMap<TerceroEditDTO, TerceroInformacionFiscal>()
    .ForMember(dest => dest.TerceroId, opt => opt.Ignore()) // ya está asignado
    .ForMember(dest => dest.Tercero, opt => opt.Ignore());
            CreateMap<TerceroEditDTO, TerceroCategoria>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())           // Ignorar PK de la tabla
    .ForMember(dest => dest.TerceroId, opt => opt.Ignore())   // Se asigna en el servicio
    .ForMember(dest => dest.ColegioId, opt => opt.Ignore())   // No quieres cambiarlo
    .ForMember(dest => dest.Tercero, opt => opt.Ignore())     // Relación
    .ForMember(dest => dest.RegimenIva, opt => opt.Ignore()); // Relación

        }
    }
}
