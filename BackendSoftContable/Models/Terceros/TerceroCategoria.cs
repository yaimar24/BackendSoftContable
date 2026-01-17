using BackendSoftContable.Models.Terceros;

namespace BackendSoftContable.Models
{
    public class TerceroCategoria : BaseEntity
    {
        public int Id { get; set; }

        // FK al tercero
        public Guid TerceroId { get; set; }
        public Tercero Tercero { get; set; } = null!;

        // FK al colegio (tenant)
        public Guid ColegioId { get; set; }
        // public Colegio Colegio { get; set; } = null!; // opcional si tienes tabla Colegio

        // FK a la tabla Categoria
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        // Régimen IVA
        public int RegimenIvaId { get; set; }
        public RegimenIva RegimenIva { get; set; } = null!;

        // Datos específicos del colegio
        public int CiudadId { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;
    }
}
