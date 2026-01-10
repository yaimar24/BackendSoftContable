namespace BackendSoftContable.Models
{
    public class TipoIdentificacion
    {
        public int Id { get; set; }
        // Ejemplo: "NIT", "Cédula de Ciudadanía", "Cédula de Extranjería"
        public string Nombre { get; set; } = string.Empty;
        // Ejemplo: "31" para NIT, "13" para Cédula (Códigos DIAN)
        public string CodigoDian { get; set; } = string.Empty;

    }
}