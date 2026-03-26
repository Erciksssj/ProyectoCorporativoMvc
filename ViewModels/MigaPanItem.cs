namespace ProyectoCorporativoMvc.ViewModels;

public class MigaPanItem
{
    public string Texto { get; set; } = string.Empty;
    public string? Controlador { get; set; }
    public string? Accion { get; set; }
    public object? RutaValores { get; set; }
    public bool Activo { get; set; }
}
