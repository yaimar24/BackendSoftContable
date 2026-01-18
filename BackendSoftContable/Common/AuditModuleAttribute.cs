namespace BackendSoftContable.Common
{


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuditModuleAttribute : Attribute
    {
        public string Nombre { get; }
        public AuditModuleAttribute(string nombre)
        {
            Nombre = nombre;
        }
    }

}
