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
public class PermisosPerfilController : BaseController
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 5;
    private const string ClaveModulo = "SEG_PERMISOSPERFIL";

    public PermisosPerfilController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Permiso(ClaveModulo, AccionPermiso.Cualquiera)]
    public async Task<IActionResult> Index(int pagina = 1)
    {
        ViewData["Title"] = "Permisos por perfil";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Permisos Perfil", activo: true));

        var puedeConsultar = User.TienePermiso(ClaveModulo, "CONSULTAR");
        var puedeGestionar = User.EsAdministrador() || User.TienePermiso(ClaveModulo, "AGREGAR") || User.TienePermiso(ClaveModulo, "EDITAR");

        ViewBag.PuedeConsultar = puedeConsultar;
        ViewBag.PuedeGestionar = puedeGestionar;

        IQueryable<PerfilPermisosResumenViewModel> consulta = _context.Perfiles
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new PerfilPermisosResumenViewModel
            {
                IdPerfil = x.Id,
                NombrePerfil = x.StrNombrePerfil,
                EsAdministrador = x.BitAdministrador,
                TotalModulosConPermisos = x.PermisosPerfil.Count(p => p.BitAgregar || p.BitEditar || p.BitConsulta || p.BitEliminar || p.BitDetalle)
            });

        if (!puedeConsultar && !puedeGestionar && !User.EsAdministrador())
        {
            consulta = consulta.Where(x => false);
        }

        var resultado = await ResultadoPaginado<PerfilPermisosResumenViewModel>.CrearAsync(consulta, pagina, PageSize);
        return View(resultado);
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Cualquiera)]
    public async Task<IActionResult> Gestionar(int idPerfil, bool soloLectura = false)
    {
        var perfil = await _context.Perfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idPerfil);
        if (perfil is null) return NotFound();

        var puedeGestionar = User.EsAdministrador() || User.TienePermiso(ClaveModulo, "AGREGAR") || User.TienePermiso(ClaveModulo, "EDITAR");
        var modelo = await ConstruirMatrizAsync(idPerfil, null, soloLectura || !puedeGestionar);

        ViewData["Title"] = puedeGestionar && !soloLectura ? "Editar permisos" : "Detalle de permisos";
        EstablecerMigas(
            Miga("Inicio", "Inicio", "Index"),
            Miga("Seguridad"),
            Miga("Permisos Perfil", "PermisosPerfil", "Index"),
            Miga(puedeGestionar && !soloLectura ? "Editar permisos" : "Detalle", activo: true));

        return View(modelo);
    }

    [HttpPost]
    [Permiso(ClaveModulo, AccionPermiso.Cualquiera)]
    public async Task<IActionResult> Gestionar(PermisosPerfilMatrizViewModel model, string operacion = "guardar")
    {
        var perfil = await _context.Perfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.IdPerfil);
        if (perfil is null) return NotFound();

        var puedeGestionar = User.EsAdministrador() || User.TienePermiso(ClaveModulo, "AGREGAR") || User.TienePermiso(ClaveModulo, "EDITAR");
        if (!puedeGestionar && !User.EsAdministrador()) return Forbid();

        model = await ConstruirMatrizAsync(model.IdPerfil, model, false);
        ViewData["Title"] = "Editar permisos";
        EstablecerMigas(
            Miga("Inicio", "Inicio", "Index"),
            Miga("Seguridad"),
            Miga("Permisos Perfil", "PermisosPerfil", "Index"),
            Miga("Editar permisos", activo: true));

        if (string.Equals(operacion, "marcar-todo", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var fila in model.Filas)
            {
                fila.BitAgregar = true;
                fila.BitEditar = true;
                fila.BitConsulta = true;
                fila.BitEliminar = true;
                fila.BitDetalle = true;
            }

            return View(model);
        }

        if (string.Equals(operacion, "limpiar-todo", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var fila in model.Filas)
            {
                fila.BitAgregar = false;
                fila.BitEditar = false;
                fila.BitConsulta = false;
                fila.BitEliminar = false;
                fila.BitDetalle = false;
            }

            return View(model);
        }

        if (model.Filas.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "No se encontraron módulos para actualizar.");
            return View(model);
        }

        var permisosActuales = await _context.PermisosPerfil
            .Where(x => x.IdPerfil == model.IdPerfil)
            .ToListAsync();

        var permisosPorModulo = permisosActuales.ToDictionary(x => x.IdModulo);

        foreach (var fila in model.Filas)
        {
            var tienePermisos = fila.TieneAlMenosUnPermiso();
            if (permisosPorModulo.TryGetValue(fila.IdModulo, out var permisoExistente))
            {
                if (!tienePermisos)
                {
                    _context.PermisosPerfil.Remove(permisoExistente);
                    continue;
                }

                permisoExistente.BitAgregar = fila.BitAgregar;
                permisoExistente.BitEditar = fila.BitEditar;
                permisoExistente.BitConsulta = fila.BitConsulta;
                permisoExistente.BitEliminar = fila.BitEliminar;
                permisoExistente.BitDetalle = fila.BitDetalle;
            }
            else if (tienePermisos)
            {
                _context.PermisosPerfil.Add(new PermisoPerfil
                {
                    IdPerfil = model.IdPerfil,
                    IdModulo = fila.IdModulo,
                    BitAgregar = fila.BitAgregar,
                    BitEditar = fila.BitEditar,
                    BitConsulta = fila.BitConsulta,
                    BitEliminar = fila.BitEliminar,
                    BitDetalle = fila.BitDetalle
                });
            }
        }

        await _context.SaveChangesAsync();

        var mensaje = "Permisos actualizados correctamente.";
        var perfilIdActual = User.FindFirst("perfilId")?.Value;
        if (perfilIdActual == model.IdPerfil.ToString())
        {
            mensaje += " Si cambiaste el perfil con el que tienes iniciada la sesión, vuelve a entrar para refrescar permisos.";
        }

        TempData["Exito"] = mensaje;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Detalle)]
    public async Task<IActionResult> Details(int id)
    {
        var permiso = await _context.PermisosPerfil.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (permiso is null) return NotFound();
        return RedirectToAction(nameof(Gestionar), new { idPerfil = permiso.IdPerfil, soloLectura = true });
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Editar)]
    public async Task<IActionResult> Edit(int id)
    {
        var permiso = await _context.PermisosPerfil.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (permiso is null) return NotFound();
        return RedirectToAction(nameof(Gestionar), new { idPerfil = permiso.IdPerfil });
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Agregar)]
    public IActionResult Create()
    {
        TempData["Info"] = "Ahora los permisos se administran desde la matriz por perfil.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Permiso(ClaveModulo, AccionPermiso.Agregar)]
    public IActionResult Create(PermisoPerfil model)
    {
        TempData["Info"] = "Ahora los permisos se administran desde la matriz por perfil.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Eliminar)]
    public async Task<IActionResult> Delete(int id)
    {
        var permiso = await _context.PermisosPerfil.Include(x => x.Modulo).Include(x => x.Perfil).FirstOrDefaultAsync(x => x.Id == id);
        if (permiso is null) return NotFound();
        ViewData["Title"] = "Eliminar permiso";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Permisos Perfil", "PermisosPerfil", "Index"), Miga("Eliminar", activo: true));
        return View(permiso);
    }

    [HttpPost, ActionName("Delete")]
    [Permiso(ClaveModulo, AccionPermiso.Eliminar)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var permiso = await _context.PermisosPerfil.FindAsync(id);
        if (permiso is null) return NotFound();
        _context.PermisosPerfil.Remove(permiso);
        await _context.SaveChangesAsync();
        TempData["Exito"] = "Permiso eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<PermisosPerfilMatrizViewModel> ConstruirMatrizAsync(int idPerfil, PermisosPerfilMatrizViewModel? estadoActual, bool soloLectura)
    {
        var perfil = await _context.Perfiles.AsNoTracking().FirstAsync(x => x.Id == idPerfil);
        var modulos = await _context.Modulos
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new { x.Id, x.StrNombreModulo })
            .ToListAsync();

        var permisosGuardados = await _context.PermisosPerfil
            .AsNoTracking()
            .Where(x => x.IdPerfil == idPerfil)
            .ToDictionaryAsync(x => x.IdModulo);

        var filasActuales = estadoActual?.Filas?.ToDictionary(x => x.IdModulo) ?? new Dictionary<int, PermisoModuloFilaViewModel>();

        var model = new PermisosPerfilMatrizViewModel
        {
            IdPerfil = perfil.Id,
            NombrePerfil = perfil.StrNombrePerfil,
            EsAdministrador = perfil.BitAdministrador,
            SoloLectura = soloLectura,
            Filas = new List<PermisoModuloFilaViewModel>()
        };

        foreach (var modulo in modulos)
        {
            if (filasActuales.TryGetValue(modulo.Id, out var filaActual))
            {
                filaActual.NombreModulo = modulo.StrNombreModulo;
                model.Filas.Add(filaActual);
                continue;
            }

            permisosGuardados.TryGetValue(modulo.Id, out var permiso);
            model.Filas.Add(new PermisoModuloFilaViewModel
            {
                IdModulo = modulo.Id,
                NombreModulo = modulo.StrNombreModulo,
                BitAgregar = permiso?.BitAgregar ?? false,
                BitEditar = permiso?.BitEditar ?? false,
                BitConsulta = permiso?.BitConsulta ?? false,
                BitEliminar = permiso?.BitEliminar ?? false,
                BitDetalle = permiso?.BitDetalle ?? false
            });
        }

        return model;
    }
}
