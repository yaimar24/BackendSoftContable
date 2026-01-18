using BackendSoftContable.DTOs.Auditoria;
using BackendSoftContable.Interfaces.Services;

namespace BackendSoftContable.Middleware
{
    public class AuditAndErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditAndErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            try
            {
                await _next(context);
                // YA NO REGISTRAMOS ÉXITO AQUÍ. Lo hace el Service para capturar DatosAntes/Después.
            }
            catch (Exception ex)
            {
                // Solo registramos si hay un error no controlado
                await auditService.LogErrorAsync(ex, new AuditContext
                {
                    Endpoint = context.Request.Path,
                    MetodoHttp = context.Request.Method
                });
                throw; // Re-lanzar para que el manejador de excepciones global responda al cliente
            }
        }
    }
}