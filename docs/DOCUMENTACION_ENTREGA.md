# Documentación de entrega

## Resumen
El sistema fue ajustado para que no dependa exclusivamente de JavaScript en el inicio de sesión. Esto permite que, si el profesor desactiva JavaScript en el navegador, el login siga funcionando mediante un captcha local de respaldo.

## Medidas de seguridad agregadas
- Validaciones server-side en formularios
- Protección CSRF automática
- Cookie HttpOnly para el JWT
- Encabezados HTTP de seguridad
- Política CSP compatible con Google reCAPTCHA
- Fallback sin JavaScript en el login

## Google reCAPTCHA
Se integró soporte para Google reCAPTCHA v2 checkbox. La verificación se hace del lado servidor usando la API `siteverify`.

### Configuración
Editar `appsettings.json`:

```json
"Recaptcha": {
  "Habilitado": true,
  "SiteKey": "TU_SITE_KEY",
  "SecretKey": "TU_SECRET_KEY",
  "UsarRecaptchaNet": false
}
```

## Comportamiento del login
- **Con JavaScript activo y reCAPTCHA configurado:** usa Google reCAPTCHA checkbox.
- **Sin JavaScript o sin llaves configuradas:** usa captcha local de respaldo.
- **Si el profesor desactiva JavaScript:** el sistema no truena y el login sigue operativo.
