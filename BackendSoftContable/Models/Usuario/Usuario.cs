using BackendSoftContable.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public int RolesId { get; set; }
    public Roles Roles { get; set; } = null!;
    // Relación con Colegio
    public int ColegioId { get; set; }
    public Colegio Colegio { get; set; } = null!;
}
