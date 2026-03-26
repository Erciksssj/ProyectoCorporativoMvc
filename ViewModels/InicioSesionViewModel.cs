using System.ComponentModel.DataAnnotations;

namespace ProyectoCorporativoMvc.ViewModels;

public class InicioSesionViewModel
{
    [Required(ErrorMessage = "Ingresa el usuario.")]
    [Display(Name = "Usuario")]
    public string StrNombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa la contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string StrPwd { get; set; } = string.Empty;

    [Display(Name = "Captcha de respaldo")]
    public string CaptchaRespuesta { get; set; } = string.Empty;

    public string CaptchaPregunta { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }

    public bool GoogleRecaptchaHabilitado { get; set; }
    public string GoogleRecaptchaSiteKey { get; set; } = string.Empty;
    public string GoogleRecaptchaScriptUrl { get; set; } = string.Empty;
    public bool JsEnabled { get; set; }
}
