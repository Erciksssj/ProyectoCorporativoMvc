using Microsoft.AspNetCore.Mvc;

namespace ProyectoCorporativoMvc.Filters;

public class PermisoAttribute : TypeFilterAttribute
{
    public PermisoAttribute(string claveModulo, AccionPermiso accion) : base(typeof(FiltroPermiso))
    {
        Arguments = new object[] { claveModulo, accion };
    }
}
