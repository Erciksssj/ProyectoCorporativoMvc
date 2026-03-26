using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ProyectoCorporativoMvc.Options;

namespace ProyectoCorporativoMvc.Services;

public class ServicioRecaptcha : IRecaptchaService
{
    private readonly HttpClient _httpClient;
    private readonly RecaptchaOptions _options;

    public ServicioRecaptcha(HttpClient httpClient, IOptions<RecaptchaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public bool EstaConfigurado => _options.EstaConfigurado();
    public string SiteKey => _options.SiteKey;
    public string ScriptUrl => _options.UsarRecaptchaNet
        ? "https://www.recaptcha.net/recaptcha/api.js"
        : "https://www.google.com/recaptcha/api.js";

    public async Task<(bool EsValido, string Mensaje)> ValidarAsync(string? token, string? remoteIp = null)
    {
        if (!EstaConfigurado)
        {
            return (false, "Google reCAPTCHA no está configurado en appsettings.json.");
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return (false, "Completa la verificación de Google reCAPTCHA.");
        }

        var endpoint = _options.UsarRecaptchaNet
            ? "https://www.recaptcha.net/recaptcha/api/siteverify"
            : "https://www.google.com/recaptcha/api/siteverify";

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["secret"] = _options.SecretKey,
                ["response"] = token,
                ["remoteip"] = remoteIp ?? string.Empty
            })
        };

        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return (false, "No fue posible validar Google reCAPTCHA en este momento.");
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            var payload = await JsonSerializer.DeserializeAsync<RecaptchaVerifyResponse>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (payload?.Success == true)
            {
                return (true, string.Empty);
            }

            var mensaje = payload?.ErrorCodes is { Length: > 0 }
                ? $"Google reCAPTCHA rechazó la verificación: {string.Join(", ", payload.ErrorCodes)}."
                : "La validación de Google reCAPTCHA no fue aceptada.";

            return (false, mensaje);
        }
        catch
        {
            return (false, "No fue posible contactar el servicio de Google reCAPTCHA.");
        }
    }

    private sealed class RecaptchaVerifyResponse
    {
        public bool Success { get; set; }
        public string[] ErrorCodes { get; set; } = [];
    }
}
