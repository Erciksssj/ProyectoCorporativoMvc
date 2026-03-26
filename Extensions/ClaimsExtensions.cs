using System.Security.Claims;

namespace ProyectoCorporativoMvc.Extensions;

public static class ClaimsExtensions
{
    public static bool EsAdministrador(this ClaimsPrincipal usuario)
        => usuario.HasClaim("esAdministrador", "true") || usuario.IsInRole("Administrador");

    public static bool TienePermiso(this ClaimsPrincipal usuario, string claveModulo, string accion)
    {
        if (usuario.EsAdministrador()) return true;
        var valor = $"{claveModulo}:{accion.ToUpperInvariant()}";
        return usuario.Claims.Any(c => c.Type == "permiso" && c.Value == valor);
    }

    public static bool TieneAlgunPermiso(this ClaimsPrincipal usuario, string claveModulo)
    {
        if (usuario.EsAdministrador()) return true;
        return usuario.Claims.Any(c => c.Type == "permiso" && c.Value.StartsWith($"{claveModulo}:"));
    }
}
