namespace BackendSoftContable.DTOs.Terceros
{
    public class TerceroClienteDTO
    {
        public Guid Id { get; set; }          // Para poder identificar al cliente si se necesita
        public string NombreCompleto { get; set; } = string.Empty;
        public string Identificacion { get; set; } = string.Empty;
    }
}
