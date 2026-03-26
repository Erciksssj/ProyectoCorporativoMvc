using Microsoft.EntityFrameworkCore;
using ProyectoCorporativoMvc.Models;

namespace ProyectoCorporativoMvc.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Perfil> Perfiles => Set<Perfil>();
    public DbSet<Modulo> Modulos => Set<Modulo>();
    public DbSet<PermisoPerfil> PermisosPerfil => Set<PermisoPerfil>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<MenuSistema> MenusSistema => Set<MenuSistema>();
    public DbSet<MenuModulo> MenuModulos => Set<MenuModulo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Perfil>().HasIndex(e => e.StrNombrePerfil).IsUnique();
        modelBuilder.Entity<Modulo>().HasIndex(e => e.StrNombreModulo).IsUnique();
        modelBuilder.Entity<Modulo>().HasIndex(e => e.StrClave).IsUnique();
        modelBuilder.Entity<Usuario>().HasIndex(e => e.StrNombreUsuario).IsUnique();
        modelBuilder.Entity<Usuario>().HasIndex(e => e.StrCorreo).IsUnique();
        modelBuilder.Entity<PermisoPerfil>().HasIndex(e => new { e.IdPerfil, e.IdModulo }).IsUnique();
        modelBuilder.Entity<MenuModulo>().HasIndex(e => new { e.IdMenu, e.IdModulo }).IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasOne(e => e.Perfil)
            .WithMany(p => p.Usuarios)
            .HasForeignKey(e => e.IdPerfil)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PermisoPerfil>()
            .HasOne(e => e.Perfil)
            .WithMany(p => p.PermisosPerfil)
            .HasForeignKey(e => e.IdPerfil)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PermisoPerfil>()
            .HasOne(e => e.Modulo)
            .WithMany(m => m.PermisosPerfil)
            .HasForeignKey(e => e.IdModulo)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MenuModulo>()
            .HasOne(e => e.Menu)
            .WithMany(m => m.MenuModulos)
            .HasForeignKey(e => e.IdMenu)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MenuModulo>()
            .HasOne(e => e.Modulo)
            .WithMany(m => m.MenuModulos)
            .HasForeignKey(e => e.IdModulo)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
