public class TerceroCreateDTO
{
    // --- DATOS BÁSICOS ---
    public int TipoPersonaId { get; set; }
    public int TipoIdentificacionId { get; set; }
    public string Identificacion { get; set; }
    public string? DV { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? NombreComercial { get; set; }

    // --- DATOS DE CONTACTO Y UBICACIÓN ---
    public string Email { get; set; }
    public string Telefono { get; set; }
    public string Direccion { get; set; }
    public int CiudadId { get; set; }

    // --- DATOS DE FACTURACIÓN Y ENVÍO (Imagen 2) ---
    public string? Indicativo { get; set; } = "+57";
    public string? CodigoPostal { get; set; }
    public string? ContactoNombres { get; set; }
    public string? ContactoApellidos { get; set; }
    public string? CorreoFacturacion { get; set; }

    // --- CONFIGURACIÓN INSTITUCIONAL ---
    public Guid ColegioId { get; set; }
    public int CategoriaId { get; set; }
    public int RegimenIvaId { get; set; }

    // --- RESPONSABILIDADES FISCALES (Checkboxes) ---
    // Recibimos la lista de IDs de la tabla ResponsabilidadFiscal
    public List<int> ResponsabilidadesFiscalesIds { get; set; } = new List<int>();
}