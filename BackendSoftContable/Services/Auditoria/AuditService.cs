using BackendSoftContable.Data;
using BackendSoftContable.DTOs.Auditoria;
using BackendSoftContable.Interfaces.Services;
using BackendSoftContable.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackendSoftContable.Services
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public AuditService(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task LogAsync(AuditEntry entry, AuditContext? manualContext = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                // 1. Obtener la IP Pública real (Priorizando X-Forwarded-For si existe)
                string ipAddress = GetClientIp(httpContext) ?? manualContext?.Ip ?? "0.0.0.0";

                // 2. Extraer datos del Token o contexto manual
                var user = httpContext?.User;
                Guid userId = manualContext?.UsuarioId ?? GetGuidFromClaim(user, ClaimTypes.NameIdentifier) ?? GetGuidFromClaim(user, "sub") ?? Guid.Empty;
                Guid colegioId = manualContext?.ColegioId ?? GetGuidFromClaim(user, "colegioId") ?? Guid.Empty;

                var log = new AuditoriaLog
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = userId,
                    ColegioId = colegioId,

                    // Datos de la Petición
                    MetodoHttp = httpContext?.Request.Method ?? manualContext?.MetodoHttp ?? "N/A",
                    Endpoint = httpContext?.Request.Path.Value ?? manualContext?.Endpoint ?? "INTERNAL",
                    Ip = ipAddress,
                    UserAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? manualContext?.UserAgent ?? "System",

                    // Datos del Negocio
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
                // En producción, esto se guarda en los logs del servidor (Docker, Azure, etc.)
                _logger.LogError(ex, "Fallo crítico al intentar guardar log de auditoría.");
            }
        }

        public async Task LogErrorAsync(Exception ex, AuditContext context)
        {
            await LogAsync(new AuditEntry
            {
                Accion = "ERROR",
                Modulo = "SISTEMA",
                Entidad = "EXCEPTION",
                Descripcion = ex.Message,
                Exitoso = false,
                DatosDespues = new { StackTrace = ex.StackTrace }
            }, context);
        }

        // Helper para extraer la IP real en la nube
        private string? GetClientIp(HttpContext? context)
        {
            if (context == null) return null;

            // X-Forwarded-For es la cabecera estándar que usan los Proxies (Nginx, Azure, AWS)
            var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                // Puede venir una lista de IPs (cliente, proxy1, proxy2). Tomamos la primera.
                return forwardedHeader.Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }

        private Guid? GetGuidFromClaim(ClaimsPrincipal? user, string claimType)
        {
            var val = user?.FindFirst(claimType)?.Value?.Trim('{', '}');
            return Guid.TryParse(val, out var res) ? res : null;
        }
    }
}