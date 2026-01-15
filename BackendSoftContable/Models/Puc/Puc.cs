using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSoftContable.Models
{
    [Table("Puc")] // Nombre exacto de la tabla en SQL
    public class Puc
    {
        [Key] // Define Codigo como Primary Key
        [StringLength(20)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        [Required]
        public int Nivel { get; set; }

        [StringLength(20)]
        public string? CodigoPadre { get; set; }

        [Required]
        [Column(TypeName = "char(1)")]
        public string Naturaleza { get; set; } // 'D' o 'C'

        [Required]
        public bool EsDetalle { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // --- RELACIÓN AUTORREFERENCIAL (OPCIONAL pero recomendada) ---

        [ForeignKey("CodigoPadre")]
        public virtual Puc? Padre { get; set; }

        public virtual ICollection<Puc> Hijos { get; set; } = new List<Puc>();
    }
}