namespace BackendSoftContable.DTOs
{
    public class ColegioCreateDTO
    {
        public string NombreColegio { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string ResDian { get; set; } = string.Empty;
        public DateTime FechaCertificado { get; set; }
        public string RepresentanteLegal { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;

        // Datos del usuario admin
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public string PlanSeleccionado { get; set; } = "Premium";
        public IFormFile? Logo { get; set; }
        public IFormFile? ArchivoDian { get; set; }
    }
}
