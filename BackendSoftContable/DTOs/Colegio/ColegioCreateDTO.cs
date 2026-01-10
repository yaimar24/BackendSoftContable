using BackendSoftContable.DTOs.Colegio;

public class ColegioCreateDTO
{
    // --- Datos del Colegio ---
    public string NombreColegio { get; set; } = string.Empty;
    public string Nit { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string PlanSeleccionado { get; set; } = "Premium";
    public IFormFile? Logo { get; set; }
    public bool? UsaDobleImpuesto { get; set; }
    public bool? UsaImpuestoAdValorem { get; set; }

    // --- Lista de Representantes ---
    public List<RepresentanteLegalDTO> RepresentantesLegales { get; set; } = new();

    // --- Configuración Tributaria y Ubicación (FKs del Colegio) ---
    public string Telefono { get; set; }
    public int RolesId { get; set; }
    public int CiudadId { get; set; }
    public int RegimenIvaId { get; set; }
    public int TributoId { get; set; }
    public int ResponsabilidadFiscalId { get; set; }
    public int ActividadEconomicaId { get; set; }
    public string? TarifaIca { get; set; }
    public bool? ManejaAiu { get; set; }
    public bool? IvaRetencion { get; set; }

    // --- Datos del Usuario Administrador ---
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
