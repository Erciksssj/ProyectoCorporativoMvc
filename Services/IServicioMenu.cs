using System.Security.Claims;
using ProyectoCorporativoMvc.ViewModels;

namespace ProyectoCorporativoMvc.Services;

public interface IServicioMenu
{
    Task<List<GrupoMenuViewModel>> ObtenerMenusAsync(ClaimsPrincipal usuario);
}
