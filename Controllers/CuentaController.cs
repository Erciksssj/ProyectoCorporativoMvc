using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoCorporativoMvc.Services;
using ProyectoCorporativoMvc.ViewModels;

namespace ProyectoCorporativoMvc.Controllers;

[AllowAnonymous]
public class CuentaController : BaseController
{
    private readonly IServicioAutenticacion _servicioAutenticacion;
    private readonly IServicioCaptcha _servicioCaptcha;
    private readonly IRecaptchaService _recaptchaService;

    public CuentaController(
        IServicioAutenticacion servicioAutenticacion,
        IServicioCaptcha servicioCaptcha,
        IRecaptchaService recaptchaService)
    {
        _servicioAutenticacion = servicioAutenticacion;
        _servicioCaptcha = servicioCaptcha;
        _recaptchaService = recaptchaService;
    }

    [HttpGet]
    public IActionResult Login(string? message = null, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true && string.IsNullOrWhiteSpace(message))
        {
            return RedirectToAction("Index", "Inicio");
        }

        var modelo = CrearModeloLogin(returnUrl);

        if (!string.IsNullOrWhiteSpace(message))
        {
            ViewBag.Error = message;
        }

        ViewData["Title"] = "Login";
        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> Login(InicioSesionViewModel model)
    {
        PrepararModeloLogin(model);

        if (!ModelState.IsValid)
        {
            model.CaptchaPregunta = _servicioCaptcha.GenerarCaptcha();
            return View(model);
        }

        var tokenRecaptcha = Request.Form["g-recaptcha-response"].ToString();

        if (model.GoogleRecaptchaHabilitado && model.JsEnabled)
        {
            var (esValidoRecaptcha, mensajeRecaptcha) = await _recaptchaService.ValidarAsync(tokenRecaptcha, HttpContext.Connection.RemoteIpAddress?.ToString());
            if (!esValidoRecaptcha)
            {
                ModelState.AddModelError(string.Empty, mensajeRecaptcha);
                model.CaptchaPregunta = _servicioCaptcha.GenerarCaptcha();
                return View(model);
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(model.CaptchaRespuesta))
            {
                ModelState.AddModelError(nameof(model.CaptchaRespuesta), "Resuelve el captcha de respaldo.");
                model.CaptchaPregunta = _servicioCaptcha.GenerarCaptcha();
                return View(model);
            }

            if (!_servicioCaptcha.ValidarCaptcha(model.CaptchaRespuesta))
            {
                ModelState.AddModelError(nameof(model.CaptchaRespuesta), "El captcha de respaldo es incorrecto.");
                model.CaptchaPregunta = _servicioCaptcha.GenerarCaptcha();
                return View(model);
            }
        }

        var (usuario, token, mensajeError) = await _servicioAutenticacion.IniciarSesionAsync(model.StrNombreUsuario, model.StrPwd);
        if (usuario is null || string.IsNullOrWhiteSpace(token))
        {
            ModelState.AddModelError(string.Empty, mensajeError);
            model.CaptchaPregunta = _servicioCaptcha.GenerarCaptcha();
            return View(model);
        }

        Response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(8)
        });

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        TempData["Exito"] = $"Bienvenido, {usuario.StrNombreUsuario}.";
        return RedirectToAction("Index", "Inicio");
    }

    [HttpGet]
    public IActionResult RefrescarCaptcha()
    {
        return Json(new { pregunta = _servicioCaptcha.GenerarCaptcha() });
    }

    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        TempData["Exito"] = "Sesión cerrada correctamente.";
        return RedirectToAction(nameof(Login));
    }

    private InicioSesionViewModel CrearModeloLogin(string? returnUrl)
    {
        return new InicioSesionViewModel
        {
            CaptchaPregunta = _servicioCaptcha.GenerarCaptcha(),
            ReturnUrl = returnUrl,
            GoogleRecaptchaHabilitado = _recaptchaService.EstaConfigurado,
            GoogleRecaptchaSiteKey = _recaptchaService.SiteKey,
            GoogleRecaptchaScriptUrl = _recaptchaService.ScriptUrl,
            JsEnabled = false
        };
    }

    private void PrepararModeloLogin(InicioSesionViewModel model)
    {
        model.GoogleRecaptchaHabilitado = _recaptchaService.EstaConfigurado;
        model.GoogleRecaptchaSiteKey = _recaptchaService.SiteKey;
        model.GoogleRecaptchaScriptUrl = _recaptchaService.ScriptUrl;
        model.JsEnabled = string.Equals(Request.Form[nameof(InicioSesionViewModel.JsEnabled)], "true", StringComparison.OrdinalIgnoreCase);
        ViewData["Title"] = "Login";
    }
}
