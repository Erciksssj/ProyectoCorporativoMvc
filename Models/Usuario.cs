using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCorporativoMvc.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(80, MinimumLength = 4, ErrorMessage = "El usuario debe tener entre 4 y 80 caracteres.")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "El usuario solo puede tener letras, números, punto, guion y guion bajo.")]
    [Display(Name = "Nombre de usuario")]
    public string StrNombreUsuario { get; set; } = string.Empty;

    [Display(Name = "Perfil")]
    public int IdPerfil { get; set; }

    [Required]
    [Display(Name = "Contraseña")]
    public string StrPwd { get; set; } = string.Empty;

    [Range(1, 2, ErrorMessage = "Selecciona un estado válido.")]
    [Display(Name = "Estado")]
    public int IdEstadoUsuario { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
    [StringLength(150)]
    [Display(Name = "Correo")]
    public string StrCorreo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número celular es obligatorio.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "El número celular debe tener 10 dígitos.")]
    [Display(Name = "Número celular")]
    public string StrNumeroCelular { get; set; } = string.Empty;

    [StringLength(250)]
    [Display(Name = "Ruta imagen")]
    public string? StrRutaImagen { get; set; }

    [ForeignKey(nameof(IdPerfil))]
    public Perfil? Perfil { get; set; }

    [NotMapped]
    public string EstadoTexto => IdEstadoUsuario == 1 ? "Activo" : "Inactivo";
}
