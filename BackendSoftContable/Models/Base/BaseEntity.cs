using System;

namespace BackendSoftContable.Models
{
    public abstract class BaseEntity
    {
        // Registro inicial
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public Guid? UsuarioCreacionId { get; set; }
        // Seguimiento de cambios
        public DateTime? FechaActualizacion { get; set; }
        public Guid? UsuarioActualizacionId { get; set; }
    }
}