using System.ComponentModel.DataAnnotations;

namespace BackendSoftContable.Models
{
    public class Colegio
    {
        public int Id { get; set; }
        public string NombreColegio { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string ResDian { get; set; } = string.Empty;
        public DateTime FechaCertificado { get; set; }
        public string RepresentanteLegal { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string PlanSeleccionado { get; set; } = "Premium";
        public string? LogoPath { get; set; }
        public string? ArchivoDianPath { get; set; }

        // Relación con usuarios
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
