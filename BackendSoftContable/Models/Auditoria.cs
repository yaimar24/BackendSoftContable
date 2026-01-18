using System;

namespace BackendSoftContable.Models
{
    public class AuditoriaLog : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UsuarioId { get; set; }
        public Guid? ColegioId { get; set; }

        public string MetodoHttp { get; set; }
        public string Endpoint { get; set; }
        public string Accion { get; set; }
        public string Modulo { get; set; }
        public string Entidad { get; set; }
        public string Descripcion { get; set; }
        public string DatosAntes { get; set; }
        public string DatosDespues { get; set; }

        public bool Exitoso { get; set; }

        // Solo para errores
        public string ErrorMensaje { get; set; }
        public string StackTrace { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
    }
}
