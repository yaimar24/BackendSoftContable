namespace BackendSoftContable.DTOs.Auditoria
{
    public class AuditContext
    {
        public Guid? UsuarioId { get; set; }
        public Guid? ColegioId { get; set; }

        public string? MetodoHttp { get; set; }
        public string?Endpoint { get; set; }
        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
    }
}

