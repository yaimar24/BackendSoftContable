using System.ComponentModel.DataAnnotations;

namespace BackendSoftContable.DTOs.Puc
{
    public class PucCreateDTO
    {
        [Required] public string Codigo { get; set; } = null!;
        [Required] public string Nombre { get; set; } = null!;
        public string? CodigoPadre { get; set; }
        [Required] public string Naturaleza { get; set; } = null!;
        public bool EsDetalle { get; set; }
        public Guid ColegioId { get; set; }
    }

    public class PucNodoDTO
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int Nivel { get; set; }
        public bool EsDetalle { get; set; }
        public string Naturaleza { get; set; } = null!;

        public Guid ColegioId { get; set; }
        public List<PucNodoDTO> Hijos { get; set; } = new List<PucNodoDTO>();
    }
}