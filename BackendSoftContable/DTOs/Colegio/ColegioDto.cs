namespace BackendSoftContable.DTOs.Colegio
{
    public class ColegioDTO
    {
        public Guid Id { get; set; }
        public string NombreColegio { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string PlanSeleccionado { get; set; } = "Premium";
        public string? LogoPath { get; set; }
    }
}
