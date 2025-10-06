using Microsoft.JSInterop;

namespace FootballerWeb.Services
{
    public interface IAuthStateService
    {
        event Action<bool>? AuthStateChanged;
        bool IsAuthenticated { get; }
        string? CurrentToken { get; }
        Task InitializeAsync();
        Task SetAuthenticatedAsync(string token);
        Task SetUnauthenticatedAsync();
        Task<bool> CheckAuthenticationAsync();
        string? GetUserEmail();
        string? GetUserName();
        string? GetUserPicture();
    }

    public class AuthStateService : IAuthStateService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IJwtClientService _jwtClientService;
        private readonly ILogger<AuthStateService> _logger;
        
        public event Action<bool>? AuthStateChanged;
        
        private bool _isAuthenticated = false;
        private string? _currentToken = null;

        public bool IsAuthenticated 
        { 
            get => _isAuthenticated;
            private set
            {
                if (_isAuthenticated != value)
                {
                    _isAuthenticated = value;
                    AuthStateChanged?.Invoke(value);
                }
            }
        }

        public string? CurrentToken => _currentToken;

        public AuthStateService(
            IJSRuntime jsRuntime, 
            IJwtClientService jwtClientService,
            ILogger<AuthStateService> logger)
        {
            _jsRuntime = jsRuntime;
            _jwtClientService = jwtClientService;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing AuthStateService...");
                await CheckAuthenticationAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing AuthStateService");
            }
        }

        public async Task<bool> CheckAuthenticationAsync()
        {
            try
            {
                // Intentar obtener el token desde localStorage
                var token = await GetTokenFromStorageAsync();
                
                if (string.IsNullOrEmpty(token))
                {
                    await SetUnauthenticatedAsync();
                    return false;
                }

                // Verificar si el token es válido
                if (_jwtClientService.IsTokenValid(token))
                {
                    _currentToken = token;
                    IsAuthenticated = true;
                    _logger.LogInformation("User is authenticated with valid token");
                    return true;
                }
                else
                {
                    _logger.LogWarning("Token found but is invalid or expired");
                    await SetUnauthenticatedAsync();
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking authentication status");
                await SetUnauthenticatedAsync();
                return false;
            }
        }

        public async Task SetAuthenticatedAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Attempted to set authentication with empty token");
                    return;
                }

                // Verificar que el token sea válido antes de guardarlo
                if (!_jwtClientService.IsTokenValid(token))
                {
                    _logger.LogWarning("Attempted to set authentication with invalid token");
                    return;
                }

                // Guardar en localStorage
                await SaveTokenToStorageAsync(token);
                
                _currentToken = token;
                IsAuthenticated = true;
                
                _logger.LogInformation("User authenticated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting authenticated state");
            }
        }

        public async Task SetUnauthenticatedAsync()
        {
            try
            {
                // Limpiar localStorage
                await RemoveTokenFromStorageAsync();
                
                _currentToken = null;
                IsAuthenticated = false;
                
                _logger.LogInformation("User logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting unauthenticated state");
            }
        }

        public string? GetUserEmail()
        {
            if (string.IsNullOrEmpty(_currentToken))
                return null;
                
            return _jwtClientService.GetUserEmailFromToken(_currentToken);
        }

        public string? GetUserName()
        {
            if (string.IsNullOrEmpty(_currentToken))
                return null;
                
            return _jwtClientService.GetUserNameFromToken(_currentToken);
        }

        public string? GetUserPicture()
        {
            if (string.IsNullOrEmpty(_currentToken))
                return null;

            try
            {
                var claims = _jwtClientService.GetAllClaimsFromToken(_currentToken);
                return claims.TryGetValue("Foto de Perfil", out var picture) ? picture : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user picture from token");
                return null;
            }
        }

        private async Task<string?> GetTokenFromStorageAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting token from localStorage");
                return null;
            }
        }

        private async Task SaveTokenToStorageAsync(string token)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving token to localStorage");
            }
        }

        private async Task RemoveTokenFromStorageAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing token from localStorage");
            }
        }
    }
}