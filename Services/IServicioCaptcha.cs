namespace ProyectoCorporativoMvc.Services;

public interface IServicioCaptcha
{
    string GenerarCaptcha();
    bool ValidarCaptcha(string respuesta);
}
