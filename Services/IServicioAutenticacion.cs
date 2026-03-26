using ProyectoCorporativoMvc.Models;

namespace ProyectoCorporativoMvc.Services;

public interface IServicioAutenticacion
{
    Task<(Usuario? Usuario, string? Token, string MensajeError)> IniciarSesionAsync(string usuario, string password);
}
