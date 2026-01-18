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

                // --- LÓGICA DE IP ---
                // Prioridad 1: IP manual (enviada desde el servicio)
                // Prioridad 2: IP detectada por proxy (nube)
                // Prioridad 3: IP de conexión directa
                string ipFinal = manualContext?.Ip ?? GetClientIp(httpContext) ?? "0.0.0.0";

                // --- LÓGICA DE IDENTIDAD ---
                var user = httpContext?.User;
                Guid userId = manualContext?.UsuarioId ?? GetGuidFromClaim(user, ClaimTypes.NameIdentifier) ?? GetGuidFromClaim(user, "sub") ?? Guid.Empty;
                Guid colegioId = manualContext?.ColegioId ?? GetGuidFromClaim(user, "colegioId") ?? Guid.Empty;

                var log = new AuditoriaLog
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = userId,
                    ColegioId = colegioId,

                    // Datos técnicos: Preferimos manualContext si el HttpContext ya expiró en el hilo asíncrono
                    MetodoHttp = manualContext?.MetodoHttp ?? httpContext?.Request.Method ?? "N/A",
                    Endpoint = manualContext?.Endpoint ?? httpContext?.Request.Path.Value ?? "INTERNAL",
                    Ip = ipFinal,
                    UserAgent = manualContext?.UserAgent ?? httpContext?.Request.Headers["User-Agent"].ToString() ?? "System",

                    // Datos de negocio
                    Accion = entry.Accion ?? "OP",
                    Modulo = entry.Modulo ?? "GENERAL",
                    Entidad = entry.Entidad ?? "N/A",
                    Descripcion = entry.Descripcion,
                    DatosAntes = entry.DatosAntes != null ? JsonSerializer.Serialize(entry.DatosAntes, JsonOptions) : null,
                    DatosDespues = entry.DatosDespues != null ? JsonSerializer.Serialize(entry.DatosDespues, JsonOptions) : null,
                    Exitoso = entry.Exitoso, // Esto guardará 'false' si el login falló
                    FechaRegistro = DateTime.UtcNow
                };

                _context.AuditoriaLog.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Registro de error en producción
                _logger.LogError(ex, "Error al persistir auditoría. Acción: {Accion}, Entidad: {Entidad}", entry.Accion, entry.Entidad);
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

        private string? GetClientIp(HttpContext? context)
        {
            if (context == null) return null;

            // Revisamos cabecera de la nube/proxy primero
            var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
            {
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