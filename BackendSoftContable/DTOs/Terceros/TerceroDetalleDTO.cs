namespace BackendSoftContable.DTOs.TerceroDetalleDTO
{
    public class TerceroDetalleDTO
    {
        public Guid Id { get; set; } // ID del Tercero
        public string TipoIdentificacionNombre { get; set; } = null!;
        public string Identificacion { get; set; } = null!;
        public string? Dv { get; set; }

        // Nombre procesado (Si es empresa NombreComercial, si no Nombres + Apellidos)
        public string RazonSocial { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string Categoria { get; set; } = null!;
        public string RegimenIvaNombre { get; set; } = null!;
        public string? Telefono { get; set; }
        public bool Activo { get; set; }
    }
}