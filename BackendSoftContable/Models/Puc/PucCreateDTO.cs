namespace BackendSoftContable.Models.DTOs
{
    public class PucCreateDto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string? CodigoPadre { get; set; }
        // La naturaleza es opcional recibirla, 
        // ya que lo ideal es heredarla del padre por norma contable.
        public string? Naturaleza { get; set; }
    }
}