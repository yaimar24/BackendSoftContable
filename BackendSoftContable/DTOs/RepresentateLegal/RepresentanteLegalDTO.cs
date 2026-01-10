namespace BackendSoftContable.DTOs.Colegio
{
    public class RepresentanteLegalDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;

        public int TipoIdentificacionId { get; set; }
    }
}
