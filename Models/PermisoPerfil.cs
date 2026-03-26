using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCorporativoMvc.Models;

public class PermisoPerfil
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un módulo.")]
    [Display(Name = "Módulo")]
    public int IdModulo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un perfil.")]
    [Display(Name = "Perfil")]
    public int IdPerfil { get; set; }

    [Display(Name = "Agregar")]
    public bool BitAgregar { get; set; }

    [Display(Name = "Editar")]
    public bool BitEditar { get; set; }

    [Display(Name = "Consultar")]
    public bool BitConsulta { get; set; }

    [Display(Name = "Eliminar")]
    public bool BitEliminar { get; set; }

    [Display(Name = "Detalle")]
    public bool BitDetalle { get; set; }

    [ForeignKey(nameof(IdModulo))]
    public Modulo? Modulo { get; set; }

    [ForeignKey(nameof(IdPerfil))]
    public Perfil? Perfil { get; set; }
}
