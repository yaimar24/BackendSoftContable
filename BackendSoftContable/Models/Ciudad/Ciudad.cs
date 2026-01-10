namespace BackendSoftContable.Models
{
    public class Ciudad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;

        // Opcional: Relación con Departamento si lo necesitas
        // public int DepartamentoId { get; set; }

        public ICollection<Colegio> Colegios { get; set; } = new List<Colegio>();
    }
}