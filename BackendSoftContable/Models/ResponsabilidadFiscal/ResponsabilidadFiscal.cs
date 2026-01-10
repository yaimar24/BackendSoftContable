using System.Collections.Generic;

namespace BackendSoftContable.Models
{
    public class ResponsabilidadFiscal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Ej: Gran contribuyente
        public string? Descripcion { get; set; } // Explicación
        public string? ImpuestosRelacionados { get; set; } // Ej: "Renta, IVA, Retenciones"
        public string? PeriodicidadDeclaracion { get; set; } // Ej: Mensual, Bimestral, Anual

        // 🔹 Relación uno a muchos: una responsabilidad fiscal puede aplicarse a varios colegios
        public ICollection<Colegio> Colegios { get; set; } = new List<Colegio>();
    }
}
