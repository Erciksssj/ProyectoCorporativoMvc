using Microsoft.AspNetCore.Http;

namespace ProyectoCorporativoMvc.Services;

public class ServicioCaptcha : IServicioCaptcha
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string SessionKey = "CAPTCHA_RESULTADO";

    public ServicioCaptcha(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerarCaptcha()
    {
        var random = new Random();
        var a = random.Next(1, 10);
        var b = random.Next(1, 10);
        _httpContextAccessor.HttpContext?.Session.SetString(SessionKey, (a + b).ToString());
        return $"¿Cuánto es {a} + {b}?";
    }

    public bool ValidarCaptcha(string respuesta)
    {
        var esperado = _httpContextAccessor.HttpContext?.Session.GetString(SessionKey);
        return !string.IsNullOrWhiteSpace(esperado) && esperado == respuesta.Trim();
    }
}
