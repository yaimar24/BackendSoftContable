using System.Collections.Generic;

namespace BackendSoftContable.Models
{
    public class Tributo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Ej: IVA, ICA
        public string? Descripcion { get; set; }

        // 🔹 Relación uno a muchos: un tributo puede estar en varios colegios
        public ICollection<Colegio> Colegios { get; set; } = new List<Colegio>();
    }
}
