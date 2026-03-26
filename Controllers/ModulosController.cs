using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCorporativoMvc.Data;
using ProyectoCorporativoMvc.Extensions;
using ProyectoCorporativoMvc.Filters;
using ProyectoCorporativoMvc.Models;
using ProyectoCorporativoMvc.ViewModels;

namespace ProyectoCorporativoMvc.Controllers;

[Authorize]
public class ModulosController : BaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 5;
    private const string ClaveModulo = "SEG_MODULOS";

    public ModulosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Permiso(ClaveModulo, AccionPermiso.Cualquiera)]
    public async Task<IActionResult> Index(string? filtro, int pagina = 1)
    {
        ViewData["Title"] = "Módulos";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Módulos", activo: true));

        ViewBag.Filtro = filtro ?? string.Empty;
        ViewBag.PuedeAgregar = User.TienePermiso(ClaveModulo, "AGREGAR");
        ViewBag.PuedeConsultar = User.TienePermiso(ClaveModulo, "CONSULTAR");
        ViewBag.PuedeEditar = User.TienePermiso(ClaveModulo, "EDITAR");
        ViewBag.PuedeEliminar = User.TienePermiso(ClaveModulo, "ELIMINAR");
        ViewBag.PuedeDetalle = User.TienePermiso(ClaveModulo, "DETALLE");

        IQueryable<Modulo> consulta = _context.Modulos.AsNoTracking().OrderBy(x => x.Id);
        if (!string.IsNullOrWhiteSpace(filtro)) consulta = consulta.Where(x => x.StrNombreModulo.Contains(filtro) || x.StrClave.Contains(filtro));
        if (!(bool)ViewBag.PuedeConsultar && !User.EsAdministrador()) consulta = consulta.Where(x => false);

        var resultado = await ResultadoPaginado<Modulo>.CrearAsync(consulta, pagina, PageSize);
        return View(resultado);
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Agregar)]
    public IActionResult Create()
    {
        ViewData["Title"] = "Crear módulo";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Módulos", "Modulos", "Index"), Miga("Crear", activo: true));
        return View(new Modulo());
    }

    [HttpPost]
    [Permiso(ClaveModulo, AccionPermiso.Agregar)]
    public async Task<IActionResult> Create(Modulo model)
    {
        if (await _context.Modulos.AnyAsync(x => x.StrNombreModulo == model.StrNombreModulo)) ModelState.AddModelError(nameof(model.StrNombreModulo), "Ya existe un módulo con ese nombre.");
        if (await _context.Modulos.AnyAsync(x => x.StrClave == model.StrClave)) ModelState.AddModelError(nameof(model.StrClave), "La clave interna ya existe.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Crear módulo";
            EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Módulos", "Modulos", "Index"), Miga("Crear", activo: true));
            return View(model);
        }

        _context.Modulos.Add(model);
        await _context.SaveChangesAsync();
        TempData["Exito"] = "Módulo creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Detalle)]
    public async Task<IActionResult> Details(int id)
    {
        var modulo = await _context.Modulos.FirstOrDefaultAsync(x => x.Id == id);
        if (modulo is null) return NotFound();
        ViewData["Title"] = "Detalle de módulo";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Módulos", "Modulos", "Index"), Miga("Detalle", activo: true));
        return View(modulo);
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Editar)]
    public async Task<IActionResult> Edit(int id)
    {
        var modulo = await _context.Modulos.FindAsync(id);
        if (modulo is null) return NotFound();
        ViewData["Title"] = "Editar módulo";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Módulos", "Modulos", "Index"), Miga("Editar", activo: true));
        return View(modulo);
    }

    [HttpPost]
    [Permiso(ClaveModulo, AccionPermiso.Editar)]
    public async Task<IActionResult> Edit(int id, Modulo model)
    {
        if (id != model.Id) return NotFound();
        if (await _context.Modulos.AnyAsync(x => x.StrNombreModulo == model.StrNombreModulo && x.Id != model.Id)) ModelState.AddModelError(nameof(model.StrNombreModulo), "Ya existe otro módulo con ese nombre.");
        if (await _context.Modulos.AnyAsync(x => x.StrClave == model.StrClave && x.Id != model.Id)) ModelState.AddModelError(nameof(model.StrClave), "La clave interna ya existe.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Editar módulo";
            EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Módulos", "Modulos", "Index"), Miga("Editar", activo: true));
            return View(model);
        }

        _context.Update(model);
        await _context.SaveChangesAsync();
        TempData["Exito"] = "Módulo actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Eliminar)]
    public async Task<IActionResult> Delete(int id)
    {
        var modulo = await _context.Modulos.FirstOrDefaultAsync(x => x.Id == id);
        if (modulo is null) return NotFound();
        ViewData["Title"] = "Eliminar módulo";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Módulos", "Modulos", "Index"), Miga("Eliminar", activo: true));
        return View(modulo);
    }

    [HttpPost, ActionName("Delete")]
    [Permiso(ClaveModulo, AccionPermiso.Eliminar)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var modulo = await _context.Modulos.FirstOrDefaultAsync(x => x.Id == id);
        if (modulo is null) return NotFound();
        _context.Modulos.Remove(modulo);
        await _context.SaveChangesAsync();
        TempData["Exito"] = "Módulo eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
