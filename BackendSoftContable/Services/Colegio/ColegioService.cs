using AutoMapper;
using BackendSoftContable.Data.Repositories;
using BackendSoftContable.DTOs;
using BackendSoftContable.DTOs.Colegio;
using BackendSoftContable.Models;

namespace BackendSoftContable.Services.Colegio;

public class ColegioService : IColegioService
{
    private readonly IColegioRepository _colegioRepo;
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IFileStorageService _fileService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;

    public ColegioService(
        IColegioRepository colegioRepo,
        IUsuarioRepository usuarioRepo,
        IFileStorageService fileService,
        IPasswordService passwordService,
        IMapper mapper)
    {
        _colegioRepo = colegioRepo;
        _usuarioRepo = usuarioRepo;
        _fileService = fileService;
        _passwordService = passwordService;
        _mapper = mapper;
    }

    public async Task<ApiResponseDTO<ColegioDTO>> RegisterAsync(ColegioCreateDTO dto)
    {
        // 1. Validaciones de negocio
        if (dto.Password != dto.ConfirmPassword)
            return new ApiResponseDTO<ColegioDTO> { Success = false, Message = "Las contraseñas no coinciden" };

        if (await _usuarioRepo.ExistsByEmailAsync(dto.Email))
            return new ApiResponseDTO<ColegioDTO> { Success = false, Message = "El email ya está registrado" };

        var colegio = _mapper.Map<BackendSoftContable.Models.Colegio>(dto);

        // 3. Procesar Logo
        if (dto.Logo != null)
            colegio.LogoPath = await _fileService.SaveAsync(dto.Logo);

       
        await _colegioRepo.AddAsync(colegio);

        // 5. Crear usuario administrador vinculado
        var admin = new Usuario
        {
            Nombre = "Administrador",
            Email = dto.Email,
            RolesId = dto.RolesId,
            PasswordHash = _passwordService.Hash(dto.Password),
            ColegioId = colegio.Id // El ID ya fue asignado por AddAsync
        };
        await _usuarioRepo.AddAsync(admin);

        return new ApiResponseDTO<ColegioDTO>
        {
            Success = true,
            Message = "Registro exitoso",
            Data = _mapper.Map<ColegioDTO>(colegio)
        };
    }
    public async Task<ColegioDetailDTO?> GetByIdAsync(int id)
    {
        var colegio = await _colegioRepo.GetByIdAsync(id);
        if (colegio == null) return null;

        return _mapper.Map<ColegioDetailDTO>(colegio);
    }

    public async Task<ApiResponseDTO<bool>> UpdateAsync(ColegioUpdateDTO dto)
    {
        var colegio = await _colegioRepo.GetByIdAsync(dto.Id);
        if (colegio == null)
        {
            return new ApiResponseDTO<bool>
            {
                Success = false,
                Message = "Colegio no encontrado"
            };
        }

        // Mapear cambios (excepto Representantes y Logo)
        _mapper.Map(dto, colegio);

        // Guardar logo si hay uno nuevo
        if (dto.Logo != null)
            colegio.LogoPath = await _fileService.SaveAsync(dto.Logo);

        // Actualizar representantes
        colegio.RepresentantesLegales.Clear();
        colegio.RepresentantesLegales = dto.RepresentantesLegales
            .Select(r => _mapper.Map<RepresentanteLegal>(r))
            .ToList();

        await _colegioRepo.UpdateAsync(colegio);

        return new ApiResponseDTO<bool>
        {
            Success = true,
            Message = "Colegio actualizado correctamente",
            Data = true
        };
    }
}
