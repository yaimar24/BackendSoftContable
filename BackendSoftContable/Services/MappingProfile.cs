using AutoMapper;
using BackendSoftContable.Models;
using BackendSoftContable.DTOs.Colegio;

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
        }
    }
}