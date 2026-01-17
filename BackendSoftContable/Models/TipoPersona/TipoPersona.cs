namespace BackendSoftContable.Models
{
    public class TipoPersona : BaseEntity
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Natural, Jurídica, etc.
        public bool Activo { get; set; } = true;
    }
}