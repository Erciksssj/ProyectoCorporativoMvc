using System.Security.Claims;

namespace ProyectoCorporativoMvc.Services;

public interface IServicioJwt
{
    string GenerarToken(IEnumerable<Claim> claims);
}
