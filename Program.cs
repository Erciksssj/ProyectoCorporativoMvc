using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoCorporativoMvc.Data;
using ProyectoCorporativoMvc.Models;
using ProyectoCorporativoMvc.Options;
using ProyectoCorporativoMvc.Services;

var builder = WebApplication.CreateBuilder(args);

Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "App_Data"));
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads", "users"));

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<RecaptchaOptions>(builder.Configuration.GetSection("Recaptcha"));

builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<Usuario>, Microsoft.AspNetCore.Identity.PasswordHasher<Usuario>>();
builder.Services.AddScoped<IServicioJwt, ServicioJwt>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioCaptcha, ServicioCaptcha>();
builder.Services.AddScoped<IServicioMenu, ServicioMenu>();
builder.Services.AddHttpClient<IRecaptchaService, ServicioRecaptcha>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("No se configuró Jwt:Key.");
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrWhiteSpace(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                var returnUrl = Uri.EscapeDataString($"{context.Request.Path}{context.Request.QueryString}");
                context.Response.Redirect($"/Cuenta/Login?message=Tu sesión no es válida o expiró.&returnUrl={returnUrl}");
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                var returnUrl = Uri.EscapeDataString($"{context.Request.Path}{context.Request.QueryString}");
                context.Response.Redirect($"/Cuenta/Login?message=No tienes permiso para acceder a esa opción.&returnUrl={returnUrl}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.InitializeAsync(db, scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseExceptionHandler("/Error/ServerError");
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";
    context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "base-uri 'self'; " +
        "object-src 'none'; " +
        "frame-ancestors 'none'; " +
        "form-action 'self'; " +
        "img-src 'self' data: https://www.google.com https://www.gstatic.com https://www.recaptcha.net; " +
        "script-src 'self' https://www.google.com https://www.gstatic.com https://www.recaptcha.net; " +
        "style-src 'self' 'unsafe-inline' https://www.gstatic.com; " +
        "frame-src 'self' https://www.google.com https://www.gstatic.com https://www.recaptcha.net; " +
        "connect-src 'self' https://www.google.com https://www.recaptcha.net;";

    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=Login}/{id?}");

app.Run();
