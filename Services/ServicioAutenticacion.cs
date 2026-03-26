using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoCorporativoMvc.Data;
using ProyectoCorporativoMvc.Models;

namespace ProyectoCorporativoMvc.Services;

public class ServicioAutenticacion : IServicioAutenticacion
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IServicioJwt _servicioJwt;

    public ServicioAutenticacion(ApplicationDbContext context, IPasswordHasher<Usuario> passwordHasher, IServicioJwt servicioJwt)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _servicioJwt = servicioJwt;
    }

    public async Task<(Usuario? Usuario, string? Token, string MensajeError)> IniciarSesionAsync(string usuario, string password)
    {
        var usuarioDb = await _context.Usuarios.Include(u => u.Perfil).FirstOrDefaultAsync(u => u.StrNombreUsuario == usuario);
        if (usuarioDb is null) return (null, null, "El usuario no existe.");
        if (usuarioDb.IdEstadoUsuario != 1) return (null, null, "El usuario está inactivo.");

        var validacion = _passwordHasher.VerifyHashedPassword(usuarioDb, usuarioDb.StrPwd, password);
        if (validacion == PasswordVerificationResult.Failed) return (null, null, "La contraseña es incorrecta.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuarioDb.Id.ToString()),
            new(ClaimTypes.Name, usuarioDb.StrNombreUsuario),
            new("perfilId", usuarioDb.IdPerfil.ToString()),
            new("esAdministrador", usuarioDb.Perfil?.BitAdministrador == true ? "true" : "false")
        };

        if (usuarioDb.Perfil?.BitAdministrador == true)
        {
            claims.Add(new(ClaimTypes.Role, "Administrador"));
        }

        var permisos = await _context.PermisosPerfil.Include(p => p.Modulo).Where(p => p.IdPerfil == usuarioDb.IdPerfil).ToListAsync();
        foreach (var permiso in permisos.Where(p => p.Modulo is not null))
        {
            var clave = permiso.Modulo!.StrClave;
            if (permiso.BitAgregar) claims.Add(new Claim("permiso", $"{clave}:AGREGAR"));
            if (permiso.BitEditar) claims.Add(new Claim("permiso", $"{clave}:EDITAR"));
            if (permiso.BitConsulta) claims.Add(new Claim("permiso", $"{clave}:CONSULTAR"));
            if (permiso.BitEliminar) claims.Add(new Claim("permiso", $"{clave}:ELIMINAR"));
            if (permiso.BitDetalle) claims.Add(new Claim("permiso", $"{clave}:DETALLE"));
        }

        var token = _servicioJwt.GenerarToken(claims);
        return (usuarioDb, token, string.Empty);
    }
}
