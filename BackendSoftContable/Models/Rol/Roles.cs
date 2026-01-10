using System.ComponentModel.DataAnnotations;

public class Roles
{
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
