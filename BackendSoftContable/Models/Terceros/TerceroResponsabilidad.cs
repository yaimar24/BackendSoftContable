using BackendSoftContable.Models.Terceros;
using BackendSoftContable.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class TerceroResponsabilidad : BaseEntity
{
    public Guid TerceroId { get; set; } // CAMBIAR DE int A Guid

    public int ResponsabilidadFiscalId { get; set; }

    [ForeignKey("ResponsabilidadFiscalId")]
    public virtual ResponsabilidadFiscal ResponsabilidadFiscal { get; set; } = null!;

    [ForeignKey("TerceroId")]
    public virtual Tercero Tercero { get; set; } = null!;
}