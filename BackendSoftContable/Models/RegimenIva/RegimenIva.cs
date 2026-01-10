namespace BackendSoftContable.Models
{
    public class RegimenIva
    {
        public int Id { get; set; }

        // Ej: "Responsable del IVA", "No responsable del IVA"
        public string Nombre { get; set; } = string.Empty;

        // Código DIAN si lo quieres usar
        public string? Codigo { get; set; }

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        // Relación
        public ICollection<Colegio> Colegios { get; set; } = new List<Colegio>();
    }
}
