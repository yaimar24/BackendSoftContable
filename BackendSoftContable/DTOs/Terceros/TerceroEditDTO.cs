namespace BackendSoftContable.DTOs.Terceros
{
    public class TerceroEditDTO
    {

        public Guid Id { get; set; }

        // DATOS BÁSICOS
        public int TipoPersonaId { get; set; }
        public int TipoIdentificacionId { get; set; }
        public string Identificacion { get; set; }
        public string? Dv { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? NombreComercial { get; set; }

        // CONTACTO
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public int? CiudadId { get; set; }

        // INFORMACIÓN FISCAL
        public string? Indicativo { get; set; }
        public string? CodigoPostal { get; set; }
        public string? ContactoNombres { get; set; }
        public string? ContactoApellidos { get; set; }
        public string? CorreoFacturacion { get; set; }

        // CONFIGURACIÓN
        public Guid ColegioId { get; set; }
        public int CategoriaId { get; set; }
        public int RegimenIvaId { get; set; }
        public bool Activo { get; set; } 

        // RESPONSABILIDADES
        public List<int> ResponsabilidadesFiscalesIds { get; set; } = new();
    }
}
