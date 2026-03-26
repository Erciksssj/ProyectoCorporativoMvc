using System.ComponentModel.DataAnnotations;

namespace ProyectoCorporativoMvc.Models;

public class Modulo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del módulo es obligatorio.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "El nombre del módulo debe tener entre 3 y 120 caracteres.")]
    [Display(Name = "Nombre del módulo")]
    public string StrNombreModulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La clave interna es obligatoria.")]
    [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "La clave interna solo puede tener letras mayúsculas, números y guion bajo.")]
    [StringLength(60)]
    [Display(Name = "Clave interna")]
    public string StrClave { get; set; } = string.Empty;

    [Required(ErrorMessage = "El controlador es obligatorio.")]
    [StringLength(60)]
    [Display(Name = "Controlador")]
    public string StrControlador { get; set; } = string.Empty;

    [Display(Name = "¿Pantalla estática?")]
    public bool BitEstatico { get; set; }

    public ICollection<PermisoPerfil> PermisosPerfil { get; set; } = new List<PermisoPerfil>();
    public ICollection<MenuModulo> MenuModulos { get; set; } = new List<MenuModulo>();
}
