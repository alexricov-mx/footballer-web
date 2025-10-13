using System.Net.Http.Headers;
using System.Text.Json;
using FootballerWeb.DTOs;
using Microsoft.JSInterop;

namespace FootballerWeb.Services;

public class LigaService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<LigaService> _logger;

    public LigaService(HttpClient httpClient, IJSRuntime jsRuntime, ILogger<LigaService> logger)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    // Obtener ligas del usuario actual
    public async Task<List<Liga>> GetMisLigasAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return new List<Liga>();

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://localhost:8090/api/ligas/mis-ligas");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                
                // El backend devuelve LigaCompleta, necesitamos mapear a Liga
                var ligasCompletas = JsonSerializer.Deserialize<List<LigaCompletaBackend>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (ligasCompletas == null) return new List<Liga>();

                // Mapear de LigaCompleta del backend a Liga del frontend
                var ligas = ligasCompletas.Select(lc => new Liga
                {
                    IdLiga = lc.IdLiga,
                    Nombre = lc.Nombre,
                    Descripcion = lc.Descripcion,
                    FechaCreacion = lc.FechaCreacion,
                    FechaInicio = lc.FechaInicio,
                    FechaFin = lc.FechaFin,
                    Estado = lc.Estado,
                    CantidadEquipos = lc.CantidadEquipos,
                    CreadorNombre = lc.CreadorNombre,
                    MiRolEnLiga = lc.MiRolEnLiga,
                    PuedoInvitar = lc.PuedoInvitar
                }).ToList();

                return ligas;
            }
            
            _logger.LogWarning("Error al obtener ligas: {StatusCode}", response.StatusCode);
            return new List<Liga>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mis ligas");
            return new List<Liga>();
        }
    }

    // Invitar usuario a liga específica
    public async Task<InvitacionLigaResponse?> InvitarUsuarioALigaAsync(int idLiga, string email, string tipoInvitacion = "jugador", string? mensaje = null)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var request = new InvitarUsuarioALigaRequest
            {
                Email = email,
                TipoInvitacion = tipoInvitacion,
                MensajePersonalizado = mensaje
            };

            var response = await _httpClient.PostAsJsonAsync($"https://localhost:8090/api/usuarios/liga/{idLiga}/invitar", request);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var invitacion = JsonSerializer.Deserialize<InvitacionLigaResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return invitacion;
            }
            
            _logger.LogWarning("Error al invitar usuario {Email} a liga {IdLiga}: {StatusCode}", email, idLiga, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al invitar usuario {Email} a liga {IdLiga}", email, idLiga);
            return null;
        }
    }

    // Obtener invitaciones pendientes de una liga
    public async Task<List<InvitacionLiga>> GetInvitacionesLigaAsync(int idLiga)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return new List<InvitacionLiga>();

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://localhost:8090/api/usuarios/liga/{idLiga}/invitaciones");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var invitaciones = JsonSerializer.Deserialize<List<InvitacionLiga>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return invitaciones ?? new List<InvitacionLiga>();
            }
            
            _logger.LogWarning("Error al obtener invitaciones de liga {IdLiga}: {StatusCode}", idLiga, response.StatusCode);
            return new List<InvitacionLiga>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener invitaciones de liga {IdLiga}", idLiga);
            return new List<InvitacionLiga>();
        }
    }

    // Crear nueva liga
    public async Task<Liga?> CrearLigaAsync(string nombre, string? descripcion = null, string estatus = "abierta", DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var request = new CrearLigaRequest
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Estatus = estatus,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:8090/api/ligas", request);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var liga = JsonSerializer.Deserialize<Liga>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return liga;
            }
            
            // Obtener el contenido del error para debug
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Error al crear liga: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear liga");
            return null;
        }
    }

    // Obtener torneos donde el usuario participa como jugador
    public async Task<List<TorneoParticipacion>> GetMisTorneosParticipandoAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return new List<TorneoParticipacion>();

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://localhost:8090/api/ligas/mis-participaciones");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var torneos = JsonSerializer.Deserialize<List<TorneoParticipacion>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return torneos ?? new List<TorneoParticipacion>();
            }
            
            _logger.LogWarning("Error al obtener mis participaciones: {StatusCode}", response.StatusCode);
            return new List<TorneoParticipacion>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mis torneos participando");
            return new List<TorneoParticipacion>();
        }
    }

    // Métodos para gestión de equipos
    public async Task<TorneoInfoDto> GetTorneoInfoAsync(int idLiga)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("No hay token de autenticación");

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://localhost:8090/api/ligas/{idLiga}/info");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var torneoInfo = JsonSerializer.Deserialize<TorneoInfoDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return torneoInfo ?? throw new Exception("Respuesta vacía del servidor");
            }

            throw new HttpRequestException($"Error en la respuesta: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener información del torneo: {ex.Message}");
            throw;
        }
    }

    public async Task<List<UsuarioSimpleDto>> GetUsuariosDisponiblesParaTorneoAsync(int idLiga)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return new List<UsuarioSimpleDto>();

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://localhost:8090/api/ligas/{idLiga}/usuarios-disponibles");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var usuarios = JsonSerializer.Deserialize<List<UsuarioSimpleDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return usuarios ?? new List<UsuarioSimpleDto>();
            }

            return new List<UsuarioSimpleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener usuarios disponibles: {ex.Message}");
            return new List<UsuarioSimpleDto>();
        }
    }

    public async Task<List<AsignacionEquipoDto>> GetAsignacionesEquiposAsync(int idLiga)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return new List<AsignacionEquipoDto>();

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://localhost:8090/api/ligas/{idLiga}/asignaciones-equipos");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var asignaciones = JsonSerializer.Deserialize<List<AsignacionEquipoDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return asignaciones ?? new List<AsignacionEquipoDto>();
            }

            return new List<AsignacionEquipoDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener asignaciones: {ex.Message}");
            return new List<AsignacionEquipoDto>();
        }
    }

    public async Task GuardarAsignacionesEquiposAsync(int idLiga, List<AsignacionEquipoDto> asignaciones)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("No hay token de autenticación");

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var request = new GuardarAsignacionesRequest { Asignaciones = asignaciones };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsync($"https://localhost:8090/api/ligas/{idLiga}/asignaciones-equipos", content);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error al guardar asignaciones: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar asignaciones: {ex.Message}");
            throw;
        }
    }

    public async Task IniciarTorneoAsync(int idLiga)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("No hay token de autenticación");

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync($"https://localhost:8090/api/ligas/{idLiga}/iniciar", null);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error al iniciar torneo: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al iniciar torneo: {ex.Message}");
            throw;
        }
    }

    public async Task<List<EquipoDetalleDto>> GetEquiposDetalleAsync(int idLiga)
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token))
                return new List<EquipoDetalleDto>();

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://localhost:8090/api/ligas/{idLiga}/equipos-detalle");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var equipos = JsonSerializer.Deserialize<List<EquipoDetalleDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return equipos ?? new List<EquipoDetalleDto>();
            }

            return new List<EquipoDetalleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener equipos detalle: {ex.Message}");
            return new List<EquipoDetalleDto>();
        }
    }
}

// DTOs para el servicio
public class InvitarUsuarioALigaRequest
{
    public string Email { get; set; } = string.Empty;
    public string? TipoInvitacion { get; set; } = "jugador";
    public string? MensajePersonalizado { get; set; }
}

// DTO que viene del backend
public class LigaCompletaBackend
{
    public int IdLiga { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Estatus { get; set; } = string.Empty; // Para compatibilidad
    public int CantidadEquipos { get; set; }
    public string CreadorNombre { get; set; } = string.Empty;
    public string MiRolEnLiga { get; set; } = string.Empty;
    public bool PuedoInvitar { get; set; }
    public bool Vigente { get; set; } = true;
}

// DTOs para gestión de equipos
public class TorneoInfoDto
{
    public int IdLiga { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int CantidadEquipos { get; set; }
    public bool EsOrganizador { get; set; }
}

public class UsuarioSimpleDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class AsignacionEquipoDto
{
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
    public int NumeroEquipo { get; set; }
}

public class GuardarAsignacionesRequest
{
    public List<AsignacionEquipoDto> Asignaciones { get; set; } = new();
}

public class EquipoDetalleDto
{
    public int NumeroEquipo { get; set; }
    public List<UsuarioEquipoDto> Usuarios { get; set; } = new();
}

public class UsuarioEquipoDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}