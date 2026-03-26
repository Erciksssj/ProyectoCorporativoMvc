using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoCorporativoMvc.Models;

namespace ProyectoCorporativoMvc.Controllers;

[AllowAnonymous]
public class ErrorController : BaseController
{
    [Route("Error/ServerError")]
    public IActionResult ServerError()
    {
        Response.StatusCode = 500;
        ViewData["Title"] = "Error del sistema";
        return View("Error", new ErrorViewModel
        {
            StatusCode = 500,
            Titulo = "Ocurrió un error inesperado",
            Mensaje = "Se produjo un problema interno. Intenta nuevamente o vuelve al inicio.",
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    [Route("Error/{statusCode:int}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        ViewData["Title"] = $"Error {statusCode}";
        var model = new ErrorViewModel
        {
            StatusCode = statusCode,
            Titulo = statusCode == 404 ? "Página no encontrada" : "Solicitud no disponible",
            Mensaje = statusCode == 404 ? "La ruta solicitada no existe o ya no está disponible." : "La solicitud no pudo completarse.",
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View("Error", model);
    }
}
