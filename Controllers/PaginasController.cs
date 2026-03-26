using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCorporativoMvc.Data;
using ProyectoCorporativoMvc.Extensions;
using ProyectoCorporativoMvc.ViewModels;

namespace ProyectoCorporativoMvc.Controllers;

[Authorize]
public class PaginasController : BaseController
{
    private readonly ApplicationDbContext _context;

    public PaginasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ModuloEstatico(string clave)
    {
        var modulo = await _context.Modulos.FirstOrDefaultAsync(x => x.StrClave == clave && x.BitEstatico);
        if (modulo is null) return NotFound();
        if (!User.TieneAlgunPermiso(clave))
        {
            return RedirectToAction("Login", "Cuenta", new { message = "No tienes permiso para acceder a esa opción.", returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString });
        }

        ViewData["Title"] = modulo.StrNombreModulo;
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga(modulo.StrNombreModulo, activo: true));

        var modelo = new PaginaEstaticaViewModel
        {
            Titulo = modulo.StrNombreModulo,
            ClaveModulo = clave,
            PuedeAgregar = User.TienePermiso(clave, "AGREGAR"),
            PuedeEditar = User.TienePermiso(clave, "EDITAR"),
            PuedeConsultar = User.TienePermiso(clave, "CONSULTAR"),
            PuedeEliminar = User.TienePermiso(clave, "ELIMINAR"),
            PuedeDetalle = User.TienePermiso(clave, "DETALLE")
        };
        return View(modelo);
    }
}
