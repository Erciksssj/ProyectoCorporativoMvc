using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoCorporativoMvc.Models;

namespace ProyectoCorporativoMvc.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context, IServiceProvider services)
    {
        await context.Database.EnsureCreatedAsync();

        if (!await context.MenusSistema.AnyAsync())
        {
            context.MenusSistema.AddRange(
                new MenuSistema { StrNombreMenu = "Seguridad", Orden = 1 },
                new MenuSistema { StrNombreMenu = "Principal 1", Orden = 2 },
                new MenuSistema { StrNombreMenu = "Principal 2", Orden = 3 }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Modulos.AnyAsync())
        {
            context.Modulos.AddRange(
                new Modulo { StrNombreModulo = "Perfil", StrClave = "SEG_PERFILES", StrControlador = "Perfiles", BitEstatico = false },
                new Modulo { StrNombreModulo = "Módulo", StrClave = "SEG_MODULOS", StrControlador = "Modulos", BitEstatico = false },
                new Modulo { StrNombreModulo = "Permisos-Perfil", StrClave = "SEG_PERMISOSPERFIL", StrControlador = "PermisosPerfil", BitEstatico = false },
                new Modulo { StrNombreModulo = "Usuario", StrClave = "SEG_USUARIOS", StrControlador = "Usuarios", BitEstatico = false },
                new Modulo { StrNombreModulo = "Principal 1.1", StrClave = "PRINCIPAL_1_1", StrControlador = "Paginas", BitEstatico = true },
                new Modulo { StrNombreModulo = "Principal 1.2", StrClave = "PRINCIPAL_1_2", StrControlador = "Paginas", BitEstatico = true },
                new Modulo { StrNombreModulo = "Principal 2.1", StrClave = "PRINCIPAL_2_1", StrControlador = "Paginas", BitEstatico = true },
                new Modulo { StrNombreModulo = "Principal 2.2", StrClave = "PRINCIPAL_2_2", StrControlador = "Paginas", BitEstatico = true }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.MenuModulos.AnyAsync())
        {
            var menus = await context.MenusSistema.OrderBy(x => x.Orden).ToListAsync();
            var modulos = await context.Modulos.OrderBy(x => x.Id).ToListAsync();
            var seguridad = menus.First(m => m.StrNombreMenu == "Seguridad");
            var principal1 = menus.First(m => m.StrNombreMenu == "Principal 1");
            var principal2 = menus.First(m => m.StrNombreMenu == "Principal 2");

            context.MenuModulos.AddRange(
                new MenuModulo { IdMenu = seguridad.Id, IdModulo = modulos.First(x => x.StrClave == "SEG_PERFILES").Id },
                new MenuModulo { IdMenu = seguridad.Id, IdModulo = modulos.First(x => x.StrClave == "SEG_MODULOS").Id },
                new MenuModulo { IdMenu = seguridad.Id, IdModulo = modulos.First(x => x.StrClave == "SEG_PERMISOSPERFIL").Id },
                new MenuModulo { IdMenu = seguridad.Id, IdModulo = modulos.First(x => x.StrClave == "SEG_USUARIOS").Id },
                new MenuModulo { IdMenu = principal1.Id, IdModulo = modulos.First(x => x.StrClave == "PRINCIPAL_1_1").Id },
                new MenuModulo { IdMenu = principal1.Id, IdModulo = modulos.First(x => x.StrClave == "PRINCIPAL_1_2").Id },
                new MenuModulo { IdMenu = principal2.Id, IdModulo = modulos.First(x => x.StrClave == "PRINCIPAL_2_1").Id },
                new MenuModulo { IdMenu = principal2.Id, IdModulo = modulos.First(x => x.StrClave == "PRINCIPAL_2_2").Id }
            );
            await context.SaveChangesAsync();
        }

        var perfilAdmin = await context.Perfiles.FirstOrDefaultAsync(p => p.StrNombrePerfil == "Administrador");
        if (perfilAdmin is null)
        {
            perfilAdmin = new Perfil { StrNombrePerfil = "Administrador", BitAdministrador = true };
            context.Perfiles.Add(perfilAdmin);
            await context.SaveChangesAsync();
        }

        var modulosBase = await context.Modulos.ToListAsync();
        foreach (var modulo in modulosBase)
        {
            var existe = await context.PermisosPerfil.AnyAsync(p => p.IdPerfil == perfilAdmin.Id && p.IdModulo == modulo.Id);
            if (!existe)
            {
                context.PermisosPerfil.Add(new PermisoPerfil
                {
                    IdPerfil = perfilAdmin.Id,
                    IdModulo = modulo.Id,
                    BitAgregar = true,
                    BitEditar = true,
                    BitConsulta = true,
                    BitEliminar = true,
                    BitDetalle = true
                });
            }
        }

        await context.SaveChangesAsync();

        if (!await context.Usuarios.AnyAsync(u => u.StrNombreUsuario == "admin"))
        {
            var hasher = services.GetRequiredService<IPasswordHasher<Usuario>>();
            var admin = new Usuario
            {
                StrNombreUsuario = "admin",
                IdPerfil = perfilAdmin.Id,
                IdEstadoUsuario = 1,
                StrCorreo = "admin@demo.local",
                StrNumeroCelular = "7710000000"
            };
            admin.StrPwd = hasher.HashPassword(admin, "Admin123*");
            context.Usuarios.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
