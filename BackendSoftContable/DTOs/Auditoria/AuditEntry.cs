namespace BackendSoftContable.DTOs.Auditoria
{
    public class AuditEntry
    {
        public string Accion { get; set; }
        public string Modulo { get; set; }
        public string Entidad { get; set; }
        public string Descripcion { get; set; }

        public object DatosAntes { get; set; }
        public object DatosDespues { get; set; }

        public bool Exitoso { get; set; } = true;
    }

}
