namespace BackendSoftContable.DTOs.Tercero
{
    public class TerceroDetalleDTO
    {
        public Guid Id { get; set; }
        public string Identificacion { get; set; } = null!;
        public string? Dv { get; set; }

        // Esta es la propiedad que le falta a tu DTO:
        public string TipoIdentificacionNombre { get; set; } = null!;

        public string RazonSocial { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Direccion { get; set; } = null!;

        public string CategoriaNombre { get; set; } = null!;
        public string RegimenIvaNombre { get; set; } = null!;
        public bool Activo { get; set; }


        public List<string> Responsabilidades { get; set; } = new List<string>();
    }
}