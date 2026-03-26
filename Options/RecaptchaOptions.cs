namespace ProyectoCorporativoMvc.Options;

public class RecaptchaOptions
{
    public bool Habilitado { get; set; }
    public string SiteKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool UsarRecaptchaNet { get; set; }

    public bool EstaConfigurado()
    {
        return Habilitado
            && !string.IsNullOrWhiteSpace(SiteKey)
            && !string.IsNullOrWhiteSpace(SecretKey);
    }
}
