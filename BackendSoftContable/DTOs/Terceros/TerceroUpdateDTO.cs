namespace BackendSoftContable.DTOs.TerceroUpdateDTO
{
    public class TerceroUpdateDTO
    {
        public int Id { get; set; } // ID de la tabla TerceroCategoria (la vinculación)
        public string? Ciudad { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public int RegimenIvaId { get; set; }
        public bool Activo { get; set; }

        // Datos del tercero que podrían cambiar
        public string Email { get; set; } = null!;
        public string? NombreComercial { get; set; }
    }
}