using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSoftContable.Models.Terceros
{
    public class TerceroInformacionFiscal : BaseEntity
    {
        [Key, ForeignKey("Tercero")]
        public Guid TerceroId { get; set; }

        public string? Indicativo { get; set; } = "+57";
        public string? CodigoPostal { get; set; }
        public string? ContactoNombres { get; set; }
        public string? ContactoApellidos { get; set; }
        public string? CorreoFacturacion { get; set; }

        public virtual Tercero Tercero { get; set; }
    }
}