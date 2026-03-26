namespace ProyectoCorporativoMvc.ViewModels;

public class GrupoMenuViewModel
{
    public string Titulo { get; set; } = string.Empty;
    public List<ItemModuloMenuViewModel> Modulos { get; set; } = new();
}

public class ItemModuloMenuViewModel
{
    public string Titulo { get; set; } = string.Empty;
    public string Controlador { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string? Clave { get; set; }
}
