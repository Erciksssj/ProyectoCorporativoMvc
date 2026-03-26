using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoCorporativoMvc.ViewModels;

public class UsuarioFormViewModel : IValidatableObject
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(80, MinimumLength = 4, ErrorMessage = "El usuario debe tener entre 4 y 80 caracteres.")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "El usuario solo puede tener letras, números, punto, guion y guion bajo.")]
    [Display(Name = "Nombre de usuario")]
    public string StrNombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona un perfil.")]
    [Display(Name = "Perfil")]
    public int IdPerfil { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string? StrPwd { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    public string? ConfirmarPwd { get; set; }

    [Range(1, 2, ErrorMessage = "Selecciona un estado válido.")]
    [Display(Name = "Estado")]
    public int IdEstadoUsuario { get; set; } = 1;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
    [StringLength(150)]
    [Display(Name = "Correo")]
    public string StrCorreo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número celular es obligatorio.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "El número celular debe tener 10 dígitos.")]
    [Display(Name = "Número celular")]
    public string StrNumeroCelular { get; set; } = string.Empty;

    [Display(Name = "Imagen del usuario")]
    public IFormFile? ImagenArchivo { get; set; }

    public string? ImagenActual { get; set; }
    public IEnumerable<SelectListItem> Perfiles { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Id.HasValue && string.IsNullOrWhiteSpace(StrPwd))
        {
            yield return new ValidationResult("La contraseña es obligatoria al crear el usuario.", new[] { nameof(StrPwd) });
        }

        if (!string.IsNullOrWhiteSpace(StrPwd))
        {
            if (StrPwd.Length < 8 || !StrPwd.Any(char.IsUpper) || !StrPwd.Any(char.IsLower) || !StrPwd.Any(char.IsDigit) || !StrPwd.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                yield return new ValidationResult("La contraseña debe tener al menos 8 caracteres e incluir mayúscula, minúscula, número y símbolo.", new[] { nameof(StrPwd) });
            }

            if (StrPwd != ConfirmarPwd)
            {
                yield return new ValidationResult("La confirmación de contraseña no coincide.", new[] { nameof(ConfirmarPwd) });
            }
        }
    }
}
