namespace BackendSoftContable.DTOs
{
    public class RegistroParametrosDTO
    {
        public object ActividadesEconomicas { get; set; } = null!;
        public object RegimenesIva { get; set; } = null!;
        public object ResponsabilidadesFiscales { get; set; } = null!;
        public object TiposIdentificacion { get; set; } = null!;
        public object Ciudades { get; set; } = null!;
        public object Tributos { get; set; } = null!;
    }
}