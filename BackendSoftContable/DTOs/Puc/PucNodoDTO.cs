namespace BackendSoftContable.DTOs.Puc
{
    public class PucNodoDTO
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int Nivel { get; set; }
        public bool EsDetalle { get; set; }
        public string Naturaleza { get; set; }
        // Esta es la clave para el árbol
        public List<PucNodoDTO> Hijos { get; set; } = new List<PucNodoDTO>();
    }
}
