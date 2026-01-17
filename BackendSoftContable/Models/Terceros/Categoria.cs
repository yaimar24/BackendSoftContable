namespace BackendSoftContable.Models
{
    public class Categoria : BaseEntity
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // 'CLIENTE', 'PROVEEDOR', 'ACUDIENTE', etc.
        public bool Activo { get; set; } = true;

        // Relación inversa (opcional)
        public ICollection<TerceroCategoria> TerceroCategorias { get; set; } = new List<TerceroCategoria>();
    }
}
