using Microsoft.AspNetCore.Mvc;
using ProyectoCorporativoMvc.ViewModels;

namespace ProyectoCorporativoMvc.Controllers;

public abstract class BaseController : Controller
{
    protected void EstablecerMigas(params MigaPanItem[] items)
    {
        ViewData["Breadcrumbs"] = items.ToList();
    }

    protected MigaPanItem Miga(string texto, string? controlador = null, string? accion = null, object? rutaValores = null, bool activo = false)
        => new() { Texto = texto, Controlador = controlador, Accion = accion, RutaValores = rutaValores, Activo = activo };
}
