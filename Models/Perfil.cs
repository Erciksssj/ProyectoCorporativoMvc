using System.ComponentModel.DataAnnotations;

namespace ProyectoCorporativoMvc.Models;

public class Perfil
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del perfil es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre del perfil debe tener entre 3 y 100 caracteres.")]
    [Display(Name = "Nombre del perfil")]
    public string StrNombrePerfil { get; set; } = string.Empty;

    [Display(Name = "¿Es administrador?")]
    public bool BitAdministrador { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<PermisoPerfil> PermisosPerfil { get; set; } = new List<PermisoPerfil>();
}
