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
public class PerfilesController : BaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 5;
    private const string ClaveModulo = "SEG_PERFILES";

    public PerfilesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Permiso(ClaveModulo, AccionPermiso.Cualquiera)]
    public async Task<IActionResult> Index(string? filtro, int pagina = 1)
    {
        ViewData["Title"] = "Perfiles";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Perfiles", activo: true));

        ViewBag.Filtro = filtro ?? string.Empty;
        ViewBag.PuedeAgregar = User.TienePermiso(ClaveModulo, "AGREGAR");
        ViewBag.PuedeConsultar = User.TienePermiso(ClaveModulo, "CONSULTAR");
        ViewBag.PuedeEditar = User.TienePermiso(ClaveModulo, "EDITAR");
        ViewBag.PuedeEliminar = User.TienePermiso(ClaveModulo, "ELIMINAR");
        ViewBag.PuedeDetalle = User.TienePermiso(ClaveModulo, "DETALLE");

        IQueryable<Perfil> consulta = _context.Perfiles.AsNoTracking().OrderBy(x => x.Id);
        if (!string.IsNullOrWhiteSpace(filtro)) consulta = consulta.Where(x => x.StrNombrePerfil.Contains(filtro));
        if (!(bool)ViewBag.PuedeConsultar && !User.EsAdministrador()) consulta = consulta.Where(x => false);

        var resultado = await ResultadoPaginado<Perfil>.CrearAsync(consulta, pagina, PageSize);
        return View(resultado);
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Agregar)]
    public IActionResult Create()
    {
        ViewData["Title"] = "Crear perfil";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Perfiles", "Perfiles", "Index"), Miga("Crear", activo: true));
        return View(new Perfil());
    }

    [HttpPost]
    [Permiso(ClaveModulo, AccionPermiso.Agregar)]
    public async Task<IActionResult> Create(Perfil model)
    {
        if (await _context.Perfiles.AnyAsync(x => x.StrNombrePerfil == model.StrNombrePerfil))
            ModelState.AddModelError(nameof(model.StrNombrePerfil), "Ya existe un perfil con ese nombre.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Crear perfil";
            EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Perfiles", "Perfiles", "Index"), Miga("Crear", activo: true));
            return View(model);
        }

        _context.Perfiles.Add(model);
        await _context.SaveChangesAsync();
        TempData["Exito"] = "Perfil creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Detalle)]
    public async Task<IActionResult> Details(int id)
    {
        var perfil = await _context.Perfiles.FirstOrDefaultAsync(x => x.Id == id);
        if (perfil is null) return NotFound();
        ViewData["Title"] = "Detalle de perfil";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Perfiles", "Perfiles", "Index"), Miga("Detalle", activo: true));
        return View(perfil);
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Editar)]
    public async Task<IActionResult> Edit(int id)
    {
        var perfil = await _context.Perfiles.FindAsync(id);
        if (perfil is null) return NotFound();
        ViewData["Title"] = "Editar perfil";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Perfiles", "Perfiles", "Index"), Miga("Editar", activo: true));
        return View(perfil);
    }

    [HttpPost]
    [Permiso(ClaveModulo, AccionPermiso.Editar)]
    public async Task<IActionResult> Edit(int id, Perfil model)
    {
        if (id != model.Id) return NotFound();
        if (await _context.Perfiles.AnyAsync(x => x.StrNombrePerfil == model.StrNombrePerfil && x.Id != model.Id))
            ModelState.AddModelError(nameof(model.StrNombrePerfil), "Ya existe otro perfil con ese nombre.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Editar perfil";
            EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Perfiles", "Perfiles", "Index"), Miga("Editar", activo: true));
            return View(model);
        }

        _context.Update(model);
        await _context.SaveChangesAsync();
        TempData["Exito"] = "Perfil actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Eliminar)]
    public async Task<IActionResult> Delete(int id)
    {
        var perfil = await _context.Perfiles.FirstOrDefaultAsync(x => x.Id == id);
        if (perfil is null) return NotFound();
        ViewData["Title"] = "Eliminar perfil";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Perfiles", "Perfiles", "Index"), Miga("Eliminar", activo: true));
        return View(perfil);
    }

    [HttpPost, ActionName("Delete")]
    [Permiso(ClaveModulo, AccionPermiso.Eliminar)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var perfil = await _context.Perfiles.Include(p => p.Usuarios).FirstOrDefaultAsync(x => x.Id == id);
        if (perfil is null) return NotFound();
        if (perfil.Usuarios.Any())
        {
            TempData["Error"] = "No puedes eliminar el perfil porque tiene usuarios asignados.";
            return RedirectToAction(nameof(Index));
        }
        _context.Perfiles.Remove(perfil);
        await _context.SaveChangesAsync();
        TempData["Exito"] = "Perfil eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
