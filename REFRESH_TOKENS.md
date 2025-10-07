# 🔄 Sistema de Refresh Tokens Automático

## ✨ Características

- ✅ **Completamente transparente**: El usuario nunca se da cuenta
- ✅ **Renovación automática**: Tokens se renuevan antes de expirar
- ✅ **Manejo de errores**: Fallback automático si falla la renovación
- ✅ **Intercepción HTTP**: Todas las requests se manejan automáticamente
- ✅ **Configuración mínima**: Una vez configurado, funciona solo

## 🏗️ Arquitectura

### Backend (API)
- **Access Token**: 15 minutos de duración (configurable)
- **Refresh Token**: 30 días de duración (configurable)
- **Middleware**: `AutoRefreshTokenMiddleware` intercepta requests automáticamente
- **Storage**: MemoryCache (reemplazable por Redis/Database)

### Frontend (Blazor)
- **JavaScript Service**: Intercepta todas las HTTP requests
- **Auto-refresh**: Renueva tokens 5 minutos antes de expirar
- **LocalStorage**: Almacena tokens de forma segura

## 🚀 Configuración

### 1. Backend ya configurado ✅
Los siguientes servicios están registrados:
- `IJwtTokenService` - Genera y valida tokens
- `IRefreshTokenService` - Maneja refresh tokens
- `AutoRefreshTokenMiddleware` - Intercepta requests automáticamente

### 2. Frontend ya configurado ✅
- Script `auto-refresh-token.js` incluido
- Servicio inicializado automáticamente
- Interceptor de HTTP configurado

## 📝 Uso

### No necesitas hacer nada especial ✨

El sistema funciona automáticamente:

1. **Login**: El usuario se autentica normalmente
2. **Tokens**: Se guardan automáticamente (access + refresh)
3. **Requests**: Se interceptan y agregan el access token
4. **Renovación**: Ocurre automáticamente antes de expirar
5. **Logout**: Limpia todos los tokens

### Eventos JavaScript (opcional)

```javascript
// Escuchar cuando se renuevan tokens
window.addEventListener('tokensRefreshed', (event) => {
    console.log('Tokens renovados:', event.detail);
});

// Escuchar cuando se requiere autenticación
window.addEventListener('authRequired', () => {
    // Redirigir al login
    window.location.href = '/auth/simple-login';
});
```

## 🔧 Configuración Avanzada

### Cambiar duración de tokens

En `appsettings.json`:
```json
{
  "JwtSettings": {
    "ExpirationMinutes": 15,  // Access token
    "SecretKey": "...",
    "Issuer": "...",
    "Audience": "..."
  }
}
```

### Cambiar duración de refresh tokens

En `RefreshTokenService.cs`:
```csharp
private const int REFRESH_TOKEN_EXPIRY_DAYS = 30; // Cambiar aquí
```

### Personalizar el buffer de renovación

```javascript
window.tokenService = new AutoRefreshTokenService({
    refreshBuffer: 3 * 60 * 1000, // 3 minutos en lugar de 5
    onTokenRefresh: (tokens) => {
        // Tu lógica personalizada
    }
});
```

## 🔒 Seguridad

- ✅ Access tokens de corta duración (15 min)
- ✅ Refresh tokens almacenados de forma segura
- ✅ Revocación automática en logout
- ✅ Validación en cada request
- ✅ Headers seguros (CORS configurado)

## 🐛 Debugging

### Logs en consola del navegador:
```
✅ Auto Refresh Token Service initialized
⏰ Next token refresh scheduled for: 14:25:30
🔄 Proactive token refresh...
✅ Access token refreshed automatically
```

### Logs en el backend:
```
[INFO] Generated access token for user 12345
[INFO] Created refresh token for user 12345
[INFO] Successfully refreshed tokens for user 12345
```

## 🔄 Flujo Completo

1. Usuario hace login → Recibe access + refresh token
2. Sistema programa renovación automática (10 min antes de expirar)
3. En cada request HTTP → Access token se agrega automáticamente
4. Cuando el token está por expirar → Se renueva automáticamente
5. Si falla la renovación → Se requiere nuevo login
6. En logout → Todos los tokens se limpian

## ✅ Estado Actual

- [x] Backend: Servicios implementados
- [x] Middleware: Intercepción automática
- [x] Frontend: Servicio JavaScript
- [x] Storage: LocalStorage + MemoryCache
- [x] Error Handling: Fallbacks implementados
- [x] Logging: Debug completo
- [x] Security: Headers y validaciones

**¡El sistema está listo para usar! 🎉**

No necesitas modificar código existente. Todos tus requests HTTP funcionarán automáticamente con renovación de tokens transparente.