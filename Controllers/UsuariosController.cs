using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoCorporativoMvc.Data;
using ProyectoCorporativoMvc.Extensions;
using ProyectoCorporativoMvc.Filters;
using ProyectoCorporativoMvc.Models;
using ProyectoCorporativoMvc.ViewModels;

namespace ProyectoCorporativoMvc.Controllers;

[Authorize]
public class UsuariosController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private const int PageSize = 5;
    private const string ClaveModulo = "SEG_USUARIOS";

    public UsuariosController(ApplicationDbContext context, IPasswordHasher<Usuario> passwordHasher, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _environment = environment;
        _configuration = configuration;
    }

    [Permiso(ClaveModulo, AccionPermiso.Cualquiera)]
    public async Task<IActionResult> Index(string? filtro, int pagina = 1)
    {
        ViewData["Title"] = "Usuarios";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Usuarios", activo: true));

        ViewBag.Filtro = filtro ?? string.Empty;
        ViewBag.PuedeAgregar = User.TienePermiso(ClaveModulo, "AGREGAR");
        ViewBag.PuedeConsultar = User.TienePermiso(ClaveModulo, "CONSULTAR");
        ViewBag.PuedeEditar = User.TienePermiso(ClaveModulo, "EDITAR");
        ViewBag.PuedeEliminar = User.TienePermiso(ClaveModulo, "ELIMINAR");
        ViewBag.PuedeDetalle = User.TienePermiso(ClaveModulo, "DETALLE");

        IQueryable<Usuario> consulta = _context.Usuarios.Include(x => x.Perfil).AsNoTracking().OrderBy(x => x.Id);
        if (!string.IsNullOrWhiteSpace(filtro)) consulta = consulta.Where(x => x.StrNombreUsuario.Contains(filtro) || x.StrCorreo.Contains(filtro) || x.StrNumeroCelular.Contains(filtro));
        if (!(bool)ViewBag.PuedeConsultar && !User.EsAdministrador()) consulta = consulta.Where(x => false);

        var resultado = await ResultadoPaginado<Usuario>.CrearAsync(consulta, pagina, PageSize);
        return View(resultado);
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Agregar)]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Crear usuario";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Usuarios", "Usuarios", "Index"), Miga("Crear", activo: true));
        var model = new UsuarioFormViewModel();
        await CargarPerfilesAsync(model);
        return View(model);
    }

    [HttpPost]
    [Permiso(ClaveModulo, AccionPermiso.Agregar)]
    public async Task<IActionResult> Create(UsuarioFormViewModel model)
    {
        await CargarPerfilesAsync(model);
        if (await _context.Usuarios.AnyAsync(x => x.StrNombreUsuario == model.StrNombreUsuario)) ModelState.AddModelError(nameof(model.StrNombreUsuario), "Ya existe un usuario con ese nombre.");
        if (await _context.Usuarios.AnyAsync(x => x.StrCorreo == model.StrCorreo)) ModelState.AddModelError(nameof(model.StrCorreo), "Ya existe un usuario con ese correo.");

        var rutaImagen = await GuardarImagenAsync(model.ImagenArchivo);
        if (rutaImagen is null && model.ImagenArchivo is not null) ModelState.AddModelError(nameof(model.ImagenArchivo), "La imagen debe ser JPG, JPEG, PNG o WEBP y no exceder 2 MB.");

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Crear usuario";
            EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Usuarios", "Usuarios", "Index"), Miga("Crear", activo: true));
            return View(model);
        }

        var usuario = new Usuario
        {
            StrNombreUsuario = model.StrNombreUsuario,
            IdPerfil = model.IdPerfil,
            IdEstadoUsuario = model.IdEstadoUsuario,
            StrCorreo = model.StrCorreo,
            StrNumeroCelular = model.StrNumeroCelular,
            StrRutaImagen = rutaImagen
        };
        usuario.StrPwd = _passwordHasher.HashPassword(usuario, model.StrPwd!);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        TempData["Exito"] = "Usuario creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Detalle)]
    public async Task<IActionResult> Details(int id)
    {
        var usuario = await _context.Usuarios.Include(x => x.Perfil).FirstOrDefaultAsync(x => x.Id == id);
        if (usuario is null) return NotFound();
        ViewData["Title"] = "Detalle de usuario";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Usuarios", "Usuarios", "Index"), Miga("Detalle", activo: true));
        return View(usuario);
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Editar)]
    public async Task<IActionResult> Edit(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null) return NotFound();

        var model = new UsuarioFormViewModel
        {
            Id = usuario.Id,
            StrNombreUsuario = usuario.StrNombreUsuario,
            IdPerfil = usuario.IdPerfil,
            IdEstadoUsuario = usuario.IdEstadoUsuario,
            StrCorreo = usuario.StrCorreo,
            StrNumeroCelular = usuario.StrNumeroCelular,
            ImagenActual = usuario.StrRutaImagen
        };
        await CargarPerfilesAsync(model);

        ViewData["Title"] = "Editar usuario";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Usuarios", "Usuarios", "Index"), Miga("Editar", activo: true));
        return View(model);
    }

    [HttpPost]
    [Permiso(ClaveModulo, AccionPermiso.Editar)]
    public async Task<IActionResult> Edit(int id, UsuarioFormViewModel model)
    {
        if (id != model.Id) return NotFound();
        await CargarPerfilesAsync(model);
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null) return NotFound();

        if (await _context.Usuarios.AnyAsync(x => x.StrNombreUsuario == model.StrNombreUsuario && x.Id != id)) ModelState.AddModelError(nameof(model.StrNombreUsuario), "Ya existe otro usuario con ese nombre.");
        if (await _context.Usuarios.AnyAsync(x => x.StrCorreo == model.StrCorreo && x.Id != id)) ModelState.AddModelError(nameof(model.StrCorreo), "Ya existe otro usuario con ese correo.");

        var nuevaRuta = model.ImagenActual;
        var imagenSubida = await GuardarImagenAsync(model.ImagenArchivo);
        if (imagenSubida is null && model.ImagenArchivo is not null)
        {
            ModelState.AddModelError(nameof(model.ImagenArchivo), "La imagen debe ser JPG, JPEG, PNG o WEBP y no exceder 2 MB.");
        }
        else if (!string.IsNullOrWhiteSpace(imagenSubida))
        {
            EliminarImagenSiExiste(usuario.StrRutaImagen);
            nuevaRuta = imagenSubida;
        }

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Editar usuario";
            EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Usuarios", "Usuarios", "Index"), Miga("Editar", activo: true));
            return View(model);
        }

        usuario.StrNombreUsuario = model.StrNombreUsuario;
        usuario.IdPerfil = model.IdPerfil;
        usuario.IdEstadoUsuario = model.IdEstadoUsuario;
        usuario.StrCorreo = model.StrCorreo;
        usuario.StrNumeroCelular = model.StrNumeroCelular;
        usuario.StrRutaImagen = nuevaRuta;
        if (!string.IsNullOrWhiteSpace(model.StrPwd)) usuario.StrPwd = _passwordHasher.HashPassword(usuario, model.StrPwd);

        _context.Update(usuario);
        await _context.SaveChangesAsync();
        TempData["Exito"] = "Usuario actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Permiso(ClaveModulo, AccionPermiso.Eliminar)]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _context.Usuarios.Include(x => x.Perfil).FirstOrDefaultAsync(x => x.Id == id);
        if (usuario is null) return NotFound();
        ViewData["Title"] = "Eliminar usuario";
        EstablecerMigas(Miga("Inicio", "Inicio", "Index"), Miga("Usuarios", "Usuarios", "Index"), Miga("Eliminar", activo: true));
        return View(usuario);
    }

    [HttpPost, ActionName("Delete")]
    [Permiso(ClaveModulo, AccionPermiso.Eliminar)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null) return NotFound();
        EliminarImagenSiExiste(usuario.StrRutaImagen);
        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        TempData["Exito"] = "Usuario eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarPerfilesAsync(UsuarioFormViewModel model)
    {
        model.Perfiles = await _context.Perfiles.OrderBy(x => x.StrNombrePerfil).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.StrNombrePerfil }).ToListAsync();
    }

    private async Task<string?> GuardarImagenAsync(IFormFile? archivo)
    {
        if (archivo is null || archivo.Length == 0) return null;

        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        var pesoMaximo = (_configuration.GetValue<int>("Archivos:PesoMaximoMb", 2)) * 1024 * 1024;
        if (!extensionesPermitidas.Contains(extension) || archivo.Length > pesoMaximo) return null;

        var carpeta = Path.Combine(_environment.WebRootPath, "uploads", "users");
        Directory.CreateDirectory(carpeta);
        var nombre = $"{Guid.NewGuid():N}{extension}";
        var rutaFisica = Path.Combine(carpeta, nombre);
        using var stream = new FileStream(rutaFisica, FileMode.Create);
        await archivo.CopyToAsync(stream);
        return $"/uploads/users/{nombre}";
    }

    private void EliminarImagenSiExiste(string? rutaRelativa)
    {
        if (string.IsNullOrWhiteSpace(rutaRelativa)) return;
        var limpia = rutaRelativa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var rutaFisica = Path.Combine(_environment.WebRootPath, limpia);
        if (System.IO.File.Exists(rutaFisica)) System.IO.File.Delete(rutaFisica);
    }
}
