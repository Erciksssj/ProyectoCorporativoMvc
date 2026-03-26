using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProyectoCorporativoMvc.Extensions;

namespace ProyectoCorporativoMvc.Filters;

public class FiltroPermiso : IAuthorizationFilter
{
    private readonly string _claveModulo;
    private readonly AccionPermiso _accion;

    public FiltroPermiso(string claveModulo, AccionPermiso accion)
    {
        _claveModulo = claveModulo;
        _accion = accion;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var usuario = context.HttpContext.User;
        if (!(usuario.Identity?.IsAuthenticated ?? false))
        {
            Redireccionar(context, "Debes iniciar sesión.");
            return;
        }

        if (usuario.EsAdministrador()) return;

        var permitido = _accion switch
        {
            AccionPermiso.Cualquiera => usuario.TieneAlgunPermiso(_claveModulo),
            AccionPermiso.Agregar => usuario.TienePermiso(_claveModulo, "AGREGAR"),
            AccionPermiso.Editar => usuario.TienePermiso(_claveModulo, "EDITAR"),
            AccionPermiso.Consultar => usuario.TienePermiso(_claveModulo, "CONSULTAR"),
            AccionPermiso.Eliminar => usuario.TienePermiso(_claveModulo, "ELIMINAR"),
            AccionPermiso.Detalle => usuario.TienePermiso(_claveModulo, "DETALLE"),
            _ => false
        };

        if (!permitido) Redireccionar(context, "No tienes permiso para acceder a esa opción.");
    }

    private static void Redireccionar(AuthorizationFilterContext context, string mensaje)
    {
        var returnUrl = $"{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}";
        context.Result = new RedirectToActionResult("Login", "Cuenta", new { message = mensaje, returnUrl });
    }
}
