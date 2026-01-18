using BackendSoftContable.Data;
using BackendSoftContable.DTOs.Auditoria;
using BackendSoftContable.Interfaces.Services;
using BackendSoftContable.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackendSoftContable.Services
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public AuditService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(AuditEntry entry, AuditContext? manualContext = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                // 1. Extraer datos del Token si no vienen manuales
                var user = httpContext?.User;
                Guid? userId = manualContext?.UsuarioId ?? GetGuidFromClaim(user, ClaimTypes.NameIdentifier) ?? GetGuidFromClaim(user, "sub");
                Guid? colId = manualContext?.ColegioId ?? GetGuidFromClaim(user, "colegioId");

                var log = new AuditoriaLog
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = userId ?? Guid.Empty,
                    ColegioId = colId ?? Guid.Empty,

                    // 2. Datos de la Petición (Automáticos)
                    MetodoHttp = httpContext?.Request.Method ?? manualContext?.MetodoHttp ?? "N/A",
                    Endpoint = httpContext?.Request.Path ?? manualContext?.Endpoint ?? "INTERNAL",
                    Ip = httpContext?.Connection.RemoteIpAddress?.ToString() ?? manualContext?.Ip ?? "0.0.0.0",
                    UserAgent = httpContext?.Request.Headers["User-Agent"] ?? manualContext?.UserAgent ?? "System",

                    // 3. Datos del Negocio (Vienen del Entry)
                    Accion = entry.Accion ?? "OP",
                    Modulo = entry.Modulo ?? "GENERAL",
                    Entidad = entry.Entidad ?? "N/A",
                    Descripcion = entry.Descripcion,
                    DatosAntes = entry.DatosAntes != null ? JsonSerializer.Serialize(entry.DatosAntes, JsonOptions) : null,
                    DatosDespues = entry.DatosDespues != null ? JsonSerializer.Serialize(entry.DatosDespues, JsonOptions) : null,
                    Exitoso = entry.Exitoso,
                    FechaRegistro = DateTime.UtcNow
                };

                _context.AuditoriaLog.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Auditoría: {ex.Message}");
            }
        }

        public async Task LogErrorAsync(Exception ex, AuditContext context)
        {
            // Similar al LogAsync pero enfocado en el error
            await LogAsync(new AuditEntry
            {
                Accion = "ERROR",
                Descripcion = ex.Message,
                Exitoso = false,
                Entidad = "EXCEPTION"
            }, context);
        }

        private Guid? GetGuidFromClaim(ClaimsPrincipal? user, string claimType)
        {
            var val = user?.FindFirst(claimType)?.Value?.Trim('{', '}');
            return Guid.TryParse(val, out var res) ? res : null;
        }
    }
}