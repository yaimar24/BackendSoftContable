using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSoftContable.Models.Terceros
{
    public class Tercero : BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // --- RELACIONES A TABLAS MAESTRAS ---
        public int TipoPersonaId { get; set; }
        [ForeignKey("TipoPersonaId")]
        public virtual TipoPersona TipoPersona { get; set; } = null!;

        public int TipoIdentificacionId { get; set; }
        [ForeignKey("TipoIdentificacionId")]
        public virtual TipoIdentificacion TipoIdentificacion { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Identificacion { get; set; } = string.Empty;

        [StringLength(1)]
        public string? Dv { get; set; }

        // --- DATOS BÁSICOS ---
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? NombreComercial { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // --- PROPIEDADES DE NAVEGACIÓN (CONEXIONES) ---
        // --- ESTADO DEL TERCERO ---
        public bool Activo { get; set; } = true;

        // 1. Relación 1 a 1 con Información Fiscal (Facturación)
        public virtual TerceroInformacionFiscal InformacionFiscal { get; set; } = null!;

        // 2. Relación Muchos a Muchos con Responsabilidades Fiscales (DIAN)
        // ESTA ES LA QUE FALTABA PARA QUE EL SERVICE NO DE ERROR
        public virtual ICollection<TerceroResponsabilidad> Responsabilidades { get; set; }
            = new List<TerceroResponsabilidad>();

        // 3. Relación con Categorías/Colegios
        public virtual ICollection<TerceroCategoria> Categorias { get; set; }
            = new List<TerceroCategoria>();
    }
}