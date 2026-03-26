namespace ProyectoCorporativoMvc.ViewModels;

public class PerfilPermisosResumenViewModel
{
    public int IdPerfil { get; set; }
    public string NombrePerfil { get; set; } = string.Empty;
    public bool EsAdministrador { get; set; }
    public int TotalModulosConPermisos { get; set; }
}
