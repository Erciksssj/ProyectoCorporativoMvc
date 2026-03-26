namespace ProyectoCorporativoMvc.ViewModels;

public class PaginaEstaticaViewModel
{
    public string Titulo { get; set; } = string.Empty;
    public string ClaveModulo { get; set; } = string.Empty;
    public bool PuedeAgregar { get; set; }
    public bool PuedeEditar { get; set; }
    public bool PuedeConsultar { get; set; }
    public bool PuedeEliminar { get; set; }
    public bool PuedeDetalle { get; set; }
}
