namespace ProyectoCorporativoMvc.ViewModels;

public class PermisoModuloFilaViewModel
{
    public int IdModulo { get; set; }
    public string NombreModulo { get; set; } = string.Empty;
    public bool BitAgregar { get; set; }
    public bool BitEditar { get; set; }
    public bool BitConsulta { get; set; }
    public bool BitEliminar { get; set; }
    public bool BitDetalle { get; set; }

    public bool TieneAlMenosUnPermiso()
        => BitAgregar || BitEditar || BitConsulta || BitEliminar || BitDetalle;
}
