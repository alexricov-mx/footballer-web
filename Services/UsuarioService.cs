using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using FootballerWeb.DTOs;

namespace FootballerWeb.Services;

public class UsuarioService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(HttpClient httpClient, IJSRuntime jsRuntime, ILogger<UsuarioService> logger)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    // Obtener todos los usuarios (solo super_admin)
    public async Task<List<UsuarioDashboard>> GetUsuariosAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return new List<UsuarioDashboard>();

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://localhost:8090/api/usuarios");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var usuarios = JsonSerializer.Deserialize<List<UsuarioDashboard>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return usuarios ?? new List<UsuarioDashboard>();
            }
            
            _logger.LogWarning("Error al obtener usuarios: {StatusCode}", response.StatusCode);
            return new List<UsuarioDashboard>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de usuarios");
            return new List<UsuarioDashboard>();
        }
    }

    // Obtener detalle de un usuario específico
    public async Task<DetalleUsuario?> GetDetalleUsuarioAsync(int idUsuario)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://localhost:8090/api/usuarios/{idUsuario}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var usuario = JsonSerializer.Deserialize<DetalleUsuario>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return usuario;
            }
            
            _logger.LogWarning("Error al obtener detalle del usuario {IdUsuario}: {StatusCode}", idUsuario, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el detalle del usuario {IdUsuario}", idUsuario);
            return null;
        }
    }

    // Invitar a un usuario
    public async Task<InvitacionResponse?> InvitarUsuarioAsync(string email, string? mensaje = null)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new InvitacionRequest
            {
                Email = email,
                MensajePersonalizado = mensaje
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:8090/api/usuarios/invitar", request);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var invitacion = JsonSerializer.Deserialize<InvitacionResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return invitacion;
            }
            
            _logger.LogWarning("Error al invitar usuario {Email}: {StatusCode}", email, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al invitar al usuario {Email}", email);
            return null;
        }
    }

    // Cambiar rol de usuario
    public async Task<bool> CambiarRolUsuarioAsync(int idUsuario, int nuevoRolId)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new CambiarRolRequest
            {
                NuevoRolId = nuevoRolId
            };

            var response = await _httpClient.PutAsJsonAsync($"https://localhost:8090/api/usuarios/{idUsuario}/rol", request);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            _logger.LogWarning("Error al cambiar rol del usuario {IdUsuario}: {StatusCode}", idUsuario, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar el rol del usuario {IdUsuario}", idUsuario);
            return false;
        }
    }

    // Cambiar estado de usuario (activo/suspendido)
    public async Task<bool> CambiarEstadoUsuarioAsync(int idUsuario, string nuevoEstado)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new CambiarEstadoRequest
            {
                NuevoEstado = nuevoEstado
            };

            var response = await _httpClient.PutAsJsonAsync($"https://localhost:8090/api/usuarios/{idUsuario}/estado", request);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            _logger.LogWarning("Error al cambiar estado del usuario {IdUsuario}: {StatusCode}", idUsuario, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar el estado del usuario {IdUsuario}", idUsuario);
            return false;
        }
    }

    // Obtener rol del usuario por email
    public async Task<string?> GetUserRoleByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var response = await _httpClient.GetAsync($"https://localhost:8090/api/usuarios/rol/{email}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UserRoleResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result?.Role;
            }
            
            _logger.LogWarning("Error al obtener rol del usuario {Email}: {StatusCode}", email, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el rol del usuario {Email}", email);
            return null;
        }
    }

    // Obtener roles disponibles
    public async Task<List<RolUsuario>> GetRolesDisponiblesAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return new List<RolUsuario>();

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://localhost:8090/api/usuarios/roles");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var roles = JsonSerializer.Deserialize<List<RolUsuario>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return roles ?? new List<RolUsuario>();
            }
            
            _logger.LogWarning("Error al obtener roles disponibles: {StatusCode}", response.StatusCode);
            return new List<RolUsuario>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los roles disponibles");
            return new List<RolUsuario>();
        }
    }

    // Obtener invitaciones pendientes de usuario
    public async Task<List<InvitacionPendiente>> GetInvitacionesPendientesAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return new List<InvitacionPendiente>();

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://localhost:8090/api/usuarios/invitaciones");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var invitaciones = JsonSerializer.Deserialize<List<InvitacionPendiente>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return invitaciones ?? new List<InvitacionPendiente>();
            }
            
            _logger.LogWarning("Error al obtener invitaciones pendientes: {StatusCode}", response.StatusCode);
            return new List<InvitacionPendiente>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las invitaciones pendientes");
            return new List<InvitacionPendiente>();
        }
    }

    // Obtener invitaciones de liga pendientes del usuario actual
    public async Task<List<InvitacionLiga>> GetMisInvitacionesLigaAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return new List<InvitacionLiga>();

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://localhost:8090/api/usuarios/mis-invitaciones-liga");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var invitaciones = JsonSerializer.Deserialize<List<InvitacionLiga>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return invitaciones ?? new List<InvitacionLiga>();
            }
            
            _logger.LogWarning("Error al obtener mis invitaciones de liga: {StatusCode}", response.StatusCode);
            return new List<InvitacionLiga>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mis invitaciones de liga");
            return new List<InvitacionLiga>();
        }
    }

    // Aceptar invitación de liga
    public async Task<bool> AceptarInvitacionLigaAsync(string codigoInvitacion)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new { CodigoInvitacion = codigoInvitacion };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:8090/api/usuarios/aceptar-invitacion-liga", request);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            _logger.LogWarning("Error al aceptar invitación de liga {Codigo}: {StatusCode}", codigoInvitacion, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aceptar invitación de liga {Codigo}", codigoInvitacion);
            return false;
        }
    }

    // Rechazar invitación de liga
    public async Task<bool> RechazarInvitacionLigaAsync(string codigoInvitacion)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new { CodigoInvitacion = codigoInvitacion };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:8090/api/usuarios/rechazar-invitacion-liga", request);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            _logger.LogWarning("Error al rechazar invitación de liga {Codigo}: {StatusCode}", codigoInvitacion, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al rechazar invitación de liga {Codigo}", codigoInvitacion);
            return false;
        }
    }

    // Aceptar invitación de usuario
    public async Task<bool> AceptarInvitacionUsuarioAsync(string codigoInvitacion)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new { CodigoInvitacion = codigoInvitacion };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:8090/api/usuarios/aceptar-invitacion-usuario", request);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            _logger.LogWarning("Error al aceptar invitación de usuario {Codigo}: {StatusCode}", codigoInvitacion, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aceptar invitación de usuario {Codigo}", codigoInvitacion);
            return false;
        }
    }

    // Rechazar invitación de usuario
    public async Task<bool> RechazarInvitacionUsuarioAsync(string codigoInvitacion)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new { CodigoInvitacion = codigoInvitacion };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:8090/api/usuarios/rechazar-invitacion-usuario", request);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            _logger.LogWarning("Error al rechazar invitación de usuario {Codigo}: {StatusCode}", codigoInvitacion, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al rechazar invitación de usuario {Codigo}", codigoInvitacion);
            return false;
        }
    }
}

// DTOs para las operaciones
public class InvitacionRequest
{
    public string Email { get; set; } = string.Empty;
    public string? MensajePersonalizado { get; set; }
}

public class CambiarRolRequest
{
    public int NuevoRolId { get; set; }
}

public class CambiarEstadoRequest
{
    public string NuevoEstado { get; set; } = string.Empty;
}

public class UserRoleResponse
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Message { get; set; }
}