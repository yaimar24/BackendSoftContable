using BackendSoftContable.Models;

public class RepresentanteLegal
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string NumeroIdentificacion { get; set; } = string.Empty;

    // Relación con TipoIdentificacion (CC, CE, etc)
    public int TipoIdentificacionId { get; set; }
    public TipoIdentificacion TipoIdentificacion { get; set; } = null!;

    // 🔹 Relación con Colegio (Cada representante pertenece a un colegio)
    public int ColegioId { get; set; }
    public Colegio Colegio { get; set; } = null!;
}