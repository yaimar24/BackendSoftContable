using System.ComponentModel.DataAnnotations;

namespace BackendSoftContable.Models.TipoFactura
{
    public class TipoFactura
    {
        [Key]
        public int Id { get; set; }
        public string nombre { get; set; } = string.Empty; // 'FACTURA DE VENTA', 'FACTURA DE COMPRA', etc.
    }
}
