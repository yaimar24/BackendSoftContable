using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSoftContable.Models
{
    [Table("CuentasContables")] // Nombre claro para diferenciar del PUC maestro
    public class CuentaContable : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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

        public bool Activa { get; set; } = true;

        // Relación jerárquica
        [ForeignKey("CodigoPadre")]
        public virtual CuentaContable? Padre { get; set; }

        public virtual ICollection<CuentaContable> Hijos { get; set; } = new List<CuentaContable>();
    }
}