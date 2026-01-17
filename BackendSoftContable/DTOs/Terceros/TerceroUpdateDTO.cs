namespace BackendSoftContable.DTOs.TerceroUpdateDTO
{
    public class TerceroUpdateDTO
    {
        public Guid Id { get; set; }

        public int TipoPersonaId { get; set; }
        public int TipoIdentificacionId { get; set; }

        public string Identificacion { get; set; } = string.Empty;
        public string? Dv { get; set; }

        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? NombreComercial { get; set; }

        public string Email { get; set; } = string.Empty;

        public List<int>? ResponsabilidadesFiscalesIds { get; set; }
    }

}