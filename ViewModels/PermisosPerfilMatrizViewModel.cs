using System.ComponentModel.DataAnnotations;

namespace ProyectoCorporativoMvc.ViewModels;

public class PermisosPerfilMatrizViewModel
{
    [Range(1, int.MaxValue)]
    public int IdPerfil { get; set; }

    public string NombrePerfil { get; set; } = string.Empty;
    public bool EsAdministrador { get; set; }
    public bool SoloLectura { get; set; }
    public List<PermisoModuloFilaViewModel> Filas { get; set; } = new();
}
