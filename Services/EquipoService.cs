using FootballerWeb.DTOs;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FootballerWeb.Services
{
    public class EquipoService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public EquipoService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<List<Equipo>?> GetEquiposPorUsuarioAsync()
        {
            try
            {
                Console.WriteLine("DEBUG: Obteniendo token de localStorage...");
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("DEBUG: No hay token en localStorage");
                    return null;
                }

                Console.WriteLine($"DEBUG: Token obtenido, longitud: {token.Length}");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine("DEBUG: Llamando a API https://localhost:8090/api/equipos/mis-equipos");
                var response = await _httpClient.GetAsync("https://localhost:8090/api/equipos/mis-equipos");
                Console.WriteLine($"DEBUG: Respuesta API: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"DEBUG: JSON respuesta: {json}");
                    var equipos = JsonSerializer.Deserialize<List<Equipo>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Console.WriteLine($"DEBUG: Equipos deserializados: {equipos?.Count ?? -1}");
                    return equipos;
                }

                Console.WriteLine($"DEBUG: API retornó error: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"DEBUG: Contenido del error: {errorContent}");
                return new List<Equipo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener equipos: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Equipo>?> GetEquiposByLigaAsync(int idLiga)
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                    return null;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"https://localhost:8090/api/equipos/liga/{idLiga}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Equipo>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return new List<Equipo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener equipos de liga: {ex.Message}");
                return null;
            }
        }

        public async Task<Equipo?> CrearEquipoAsync(string nombre)
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                    return null;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new { Nombre = nombre };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://localhost:8090/api/equipos", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Equipo>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear equipo: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AsignarEquipoALigaAsync(int idEquipo, int idLiga)
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                    return false;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new { IdEquipo = idEquipo, IdLiga = idLiga };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://localhost:8090/api/equipos/asignar-liga", content);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al asignar equipo a liga: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Equipo>?> GetEquiposPorLigaAsync(int idLiga)
        {
            try
            {
                Console.WriteLine($"DEBUG: Obteniendo equipos para liga {idLiga}...");
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("DEBUG: No hay token en localStorage");
                    return null;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                Console.WriteLine($"DEBUG: Llamando a API https://localhost:8090/api/equipos/liga/{idLiga}");
                var response = await _httpClient.GetAsync($"https://localhost:8090/api/equipos/liga/{idLiga}");
                Console.WriteLine($"DEBUG: Respuesta API equipos liga: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"DEBUG: JSON equipos liga: {json}");
                    var equipos = JsonSerializer.Deserialize<List<Equipo>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Console.WriteLine($"DEBUG: Equipos de liga deserializados: {equipos?.Count ?? -1}");
                    return equipos;
                }

                Console.WriteLine($"DEBUG: API equipos liga retornó error: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"DEBUG: Contenido del error equipos liga: {errorContent}");
                return new List<Equipo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener equipos de liga {idLiga}: {ex.Message}");
                return null;
            }
        }

        // Métodos para CRUD de equipos
        public async Task<List<EquipoDto>> GetEquiposAsync()
        {
            try
            {
                Console.WriteLine("DEBUG: Obteniendo todos los equipos...");
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("DEBUG: No hay token, usando datos de prueba");
                    return GetEquiposPrueba();
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("https://localhost:8090/api/equipos");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var equipos = JsonSerializer.Deserialize<List<EquipoDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return equipos ?? new List<EquipoDto>();
                }

                Console.WriteLine($"DEBUG: API equipos retornó error: {response.StatusCode}");
                return GetEquiposPrueba();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener equipos: {ex.Message}");
                return GetEquiposPrueba();
            }
        }

        public async Task<EquipoDto> CrearEquipoAsync(EquipoFormDto equipoForm)
        {
            try
            {
                Console.WriteLine($"DEBUG: Creando equipo: {equipoForm.Nombre}");
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("No hay token de autenticación");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var json = JsonSerializer.Serialize(equipoForm);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("https://localhost:8090/api/equipos", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var equipo = JsonSerializer.Deserialize<EquipoDto>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return equipo ?? new EquipoDto { Id = 1, Nombre = equipoForm.Nombre, Descripcion = equipoForm.Descripcion };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"DEBUG: Error al crear equipo: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Error al crear equipo: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear equipo: {ex.Message}");
                throw;
            }
        }

        public async Task<EquipoDto> ActualizarEquipoAsync(int id, EquipoFormDto equipoForm)
        {
            try
            {
                Console.WriteLine($"DEBUG: Actualizando equipo {id}: {equipoForm.Nombre}");
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("No hay token de autenticación");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var json = JsonSerializer.Serialize(equipoForm);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"https://localhost:8090/api/equipos/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var equipo = JsonSerializer.Deserialize<EquipoDto>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return equipo ?? new EquipoDto { Id = id, Nombre = equipoForm.Nombre, Descripcion = equipoForm.Descripcion };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"DEBUG: Error al actualizar equipo {id}: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Error al actualizar equipo: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar equipo {id}: {ex.Message}");
                throw;
            }
        }

        public async Task EliminarEquipoAsync(int id)
        {
            try
            {
                Console.WriteLine($"DEBUG: Eliminando equipo {id}");
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("No hay token de autenticación");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.DeleteAsync($"https://localhost:8090/api/equipos/{id}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"DEBUG: Error al eliminar equipo {id}: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Error al eliminar equipo: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar equipo {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<EquipoDto>> GetEquiposDisponiblesParaTorneoAsync(int torneoId)
        {
            try
            {
                Console.WriteLine($"DEBUG: Obteniendo equipos disponibles para torneo {torneoId}");
                var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
                if (string.IsNullOrEmpty(token))
                {
                    return GetEquiposPrueba();
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"https://localhost:8090/api/equipos/disponibles-torneo/{torneoId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var equipos = JsonSerializer.Deserialize<List<EquipoDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return equipos ?? new List<EquipoDto>();
                }

                Console.WriteLine($"DEBUG: Error al obtener equipos disponibles: {response.StatusCode}");
                return GetEquiposPrueba();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener equipos disponibles para torneo {torneoId}: {ex.Message}");
                return GetEquiposPrueba();
            }
        }

        private List<EquipoDto> GetEquiposPrueba()
        {
            return new List<EquipoDto>
            {
                new EquipoDto { Id = 1, Nombre = "Real Madrid", Descripcion = "Club de fútbol profesional", FechaCreacion = DateTime.Now.AddMonths(-3) },
                new EquipoDto { Id = 2, Nombre = "Barcelona FC", Descripcion = "Club de fútbol catalán", FechaCreacion = DateTime.Now.AddMonths(-2) },
                new EquipoDto { Id = 3, Nombre = "Atlético Madrid", Descripcion = "Los Colchoneros", FechaCreacion = DateTime.Now.AddMonths(-1) },
                new EquipoDto { Id = 4, Nombre = "Valencia CF", Descripcion = "Club histórico español", FechaCreacion = DateTime.Now.AddDays(-14) },
                new EquipoDto { Id = 5, Nombre = "Sevilla FC", Descripcion = "Los Nervionenses", FechaCreacion = DateTime.Now.AddDays(-7) }
            };
        }
    }

    // DTOs para equipos
    public class EquipoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class EquipoFormDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}