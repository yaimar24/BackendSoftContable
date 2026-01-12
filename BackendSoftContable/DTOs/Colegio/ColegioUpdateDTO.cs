namespace BackendSoftContable.DTOs.Colegio
{
    public class ColegioUpdateDTO
    {
        public Guid Id { get; set; } 
        public string NombreColegio { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        // El logo es opcional en la edición
        public IFormFile? Logo { get; set; }

        public int CiudadId { get; set; }
        public int RegimenIvaId { get; set; }
        public int TributoId { get; set; }
        public int ResponsabilidadFiscalId { get; set; }
        public int ActividadEconomicaId { get; set; }

        public bool ManejaAiu { get; set; }
        public bool IvaRetencion { get; set; }
        public string TarifaIca { get; set; }
        public bool UsaDobleImpuesto { get; set; }
        public bool UsaImpuestoAdValorem { get; set; }

        // Lista de representantes para actualizar
        public List<RepresentanteLegalDTO> RepresentantesLegales { get; set; } = new();
    }
}