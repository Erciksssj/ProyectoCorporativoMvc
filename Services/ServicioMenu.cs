using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ProyectoCorporativoMvc.Data;
using ProyectoCorporativoMvc.Extensions;
using ProyectoCorporativoMvc.ViewModels;

namespace ProyectoCorporativoMvc.Services;

public class ServicioMenu : IServicioMenu
{
    private readonly ApplicationDbContext _context;

    public ServicioMenu(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GrupoMenuViewModel>> ObtenerMenusAsync(ClaimsPrincipal usuario)
    {
        if (!(usuario.Identity?.IsAuthenticated ?? false)) return new List<GrupoMenuViewModel>();

        var menus = await _context.MenusSistema
            .Include(m => m.MenuModulos)
                .ThenInclude(mm => mm.Modulo)
            .OrderBy(m => m.Orden)
            .ToListAsync();

        var resultado = new List<GrupoMenuViewModel>();
        foreach (var menu in menus)
        {
            var grupo = new GrupoMenuViewModel { Titulo = menu.StrNombreMenu };
            foreach (var item in menu.MenuModulos.OrderBy(mm => mm.Id))
            {
                if (item.Modulo is null) continue;
                if (!usuario.EsAdministrador() && !usuario.TieneAlgunPermiso(item.Modulo.StrClave)) continue;

                grupo.Modulos.Add(new ItemModuloMenuViewModel
                {
                    Titulo = item.Modulo.StrNombreModulo,
                    Controlador = item.Modulo.BitEstatico ? "Paginas" : item.Modulo.StrControlador,
                    Accion = item.Modulo.BitEstatico ? "ModuloEstatico" : "Index",
                    Clave = item.Modulo.StrClave
                });
            }
            if (grupo.Modulos.Count > 0) resultado.Add(grupo);
        }
        return resultado;
    }
}
