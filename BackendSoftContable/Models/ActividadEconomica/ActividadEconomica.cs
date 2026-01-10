using System.ComponentModel.DataAnnotations;

namespace BackendSoftContable.Models
{
    public class ActividadEconomica
    {
        public int Id { get; set; }

        [Required]
        [StringLength(4)]
        public string Codigo { get; set; } = string.Empty; // Ejemplo: "8511" (Educación inicial)

        [Required]
        public string Descripcion { get; set; } = string.Empty; // Ejemplo: "Educación de instituciones de educación básica primaria"

        // Relación inversa: Un código puede estar en muchos colegios
        public virtual ICollection<Colegio> Colegios { get; set; } = new List<Colegio>();
    }
}