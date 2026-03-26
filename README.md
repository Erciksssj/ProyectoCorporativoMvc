# Proyecto Corporativo MVC

Proyecto en ASP.NET Core MVC listo para abrir en Visual Studio 2022 o ejecutar con Docker.

## Tecnologías
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core + SQLite
- JWT almacenado en cookie HttpOnly
- Protección CSRF con antiforgery
- Seguridad adicional con encabezados HTTP
- Google reCAPTCHA v2 checkbox opcional
- Captcha local de respaldo para funcionar sin JavaScript

## Credenciales iniciales
- Usuario: `admin`
- Contraseña: `Admin123*`

## Cómo ejecutar en Visual Studio 2022
1. Abrir `ProyectoCorporativoMvc.sln`
2. Restaurar paquetes NuGet
3. Ejecutar el proyecto

## Cómo ejecutar con Docker
```bash
docker compose up --build
```

## Seguridad implementada
- Validaciones del lado servidor
- AutoValidateAntiforgeryToken en formularios POST
- JWT firmado y guardado en cookie HttpOnly
- Encabezados de seguridad: CSP, X-Frame-Options, X-Content-Type-Options, Referrer-Policy y Permissions-Policy
- El login no depende de JavaScript para funcionar
- Si JavaScript está desactivado, el sistema usa captcha local de respaldo

## Activar Google reCAPTCHA v2 checkbox
En `appsettings.json` configura esto:

```json
"Recaptcha": {
  "Habilitado": true,
  "SiteKey": "TU_SITE_KEY",
  "SecretKey": "TU_SECRET_KEY",
  "UsarRecaptchaNet": false
}
```

Cuando `Habilitado` está en `true` y agregas las llaves:
- Con JavaScript activo se muestra el checkbox **No soy un robot**.
- Con JavaScript desactivado el login sigue funcionando con captcha local.

## Nota importante
Para pruebas en localhost registra tu dominio o entorno en la consola de reCAPTCHA de Google antes de usar las llaves.
