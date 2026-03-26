namespace ProyectoCorporativoMvc.Services;

public interface IRecaptchaService
{
    bool EstaConfigurado { get; }
    string SiteKey { get; }
    string ScriptUrl { get; }
    Task<(bool EsValido, string Mensaje)> ValidarAsync(string? token, string? remoteIp = null);
}
