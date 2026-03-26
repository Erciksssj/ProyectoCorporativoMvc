using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoCorporativoMvc.Services;

namespace ProyectoCorporativoMvc.Controllers;

[Authorize]
public class InicioController : BaseController
{
    private readonly IServicioMenu _servicioMenu;

    public InicioController(IServicioMenu servicioMenu)
    {
        _servicioMenu = servicioMenu;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Inicio";
        EstablecerMigas(Miga("Inicio", activo: true));
        var menus = await _servicioMenu.ObtenerMenusAsync(User);
        return View(menus);
    }
}
