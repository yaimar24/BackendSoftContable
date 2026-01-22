using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSoftContable.Models
{
    [Table("Puc")]
    public class Puc
    {
        // La PK se define en AppDbContext como { Codigo, ColegioId }
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        // Guid.Empty (0000...) significa cuenta global del sistema
        public Guid ColegioId { get; set; } = Guid.Empty;

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public int Nivel { get; set; }

        [StringLength(20)]
        public string? CodigoPadre { get; set; }

        [Required]
        [Column(TypeName = "char(1)")]
        public string Naturaleza { get; set; } = "D"; // 'D' o 'C'

        [Required]
        public bool EsDetalle { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // --- RELACIONES ---

        [ForeignKey("ColegioId")]
        public virtual Colegio? Colegio { get; set; }

        // Relación jerárquica corregida para Llave Compuesta
        public virtual Puc? Padre { get; set; }
        public virtual ICollection<Puc> Hijos { get; set; } = new List<Puc>();
    }
}