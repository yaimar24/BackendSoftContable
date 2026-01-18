using BackendSoftContable.DTOs.Auditoria;

namespace BackendSoftContable.Interfaces.Services
{
    public interface IAuditService
    {
        Task LogAsync(AuditEntry entry, AuditContext? ctx = null);
        Task LogErrorAsync(Exception ex, AuditContext ctx);
    }
}
