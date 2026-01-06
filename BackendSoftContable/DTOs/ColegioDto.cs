namespace BackendSoftContable.DTOs
{
    public class ColegioDTO
    {
        public int Id { get; set; }
        public string NombreColegio { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string PlanSeleccionado { get; set; } = "Premium";
        public string? LogoPath { get; set; }
        public string? ArchivoDianPath { get; set; }
    }
}
