using FootballerWeb.DTOs;

namespace FootballerWeb.Services
{
    public interface IAuthService
    {
        Task<string?> GetGoogleAuthUrlAsync();
        Task<LoginResponseDto> ProcessGoogleCallbackAsync(string code, string state);
        Task<bool> ValidateTokenAsync(string token);
        bool IsUserAuthenticated();
        string? GetCurrentUserToken();
        void SetUserToken(string token);
        void ClearUserToken();
        Task<bool> LogoutAsync();
        Task<bool> CheckAuthenticationStatusAsync();
        event Action<bool>? AuthenticationStateChanged;
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private readonly IJwtClientService _jwtClientService;
        
        // In a real app, you'd use a more secure storage mechanism
        private string? _currentToken;
        
        public event Action<bool>? AuthenticationStateChanged;

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger, IJwtClientService jwtClientService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jwtClientService = jwtClientService;
        }

        public async Task<string?> GetGoogleAuthUrlAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/auth/login");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    // Extract authUrl from the response
                    return result?.GetProperty("authUrl").GetString();
                }
                
                _logger.LogError("Failed to get Google auth URL: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Google auth URL");
                return null;
            }
        }

        public Task<LoginResponseDto> ProcessGoogleCallbackAsync(string code, string state)
        {
            try
            {
                // The callback will be handled by the API directly
                // We just need to wait for the user to be redirected back with the JWT token
                // This method would be called after the user is redirected back from Google
                
                return Task.FromResult(new LoginResponseDto 
                { 
                    Success = false, 
                    Message = "This method should be replaced with direct API integration" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Google callback");
                return Task.FromResult(new LoginResponseDto 
                { 
                    Success = false, 
                    Message = "Error processing authentication callback" 
                });
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var request = new { Token = token };
                var response = await _httpClient.PostAsJsonAsync("/token/validate", request);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token with API");
                return false;
            }
        }

        public bool IsUserAuthenticated()
        {
            if (string.IsNullOrEmpty(_currentToken))
                return false;
                
            return _jwtClientService.IsTokenValid(_currentToken);
        }

        public string? GetCurrentUserToken()
        {
            return _currentToken;
        }

        public void SetUserToken(string token)
        {
            _currentToken = token;
            AuthenticationStateChanged?.Invoke(true);
        }

        public void ClearUserToken()
        {
            _currentToken = null;
            AuthenticationStateChanged?.Invoke(false);
        }

        public Task<bool> LogoutAsync()
        {
            try
            {
                // Call API logout endpoint if available
                // For now, we'll just clear the local token
                // In a full implementation, you'd have a logout endpoint on the API
                
                ClearUserToken();
                
                _logger.LogInformation("User logged out successfully locally");
                
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout process");
                
                // Still clear local token even if there's an error
                ClearUserToken();
                
                return Task.FromResult(false);
            }
        }

        public async Task<bool> CheckAuthenticationStatusAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_currentToken))
                {
                    AuthenticationStateChanged?.Invoke(false);
                    return false;
                }

                // Check if token is valid locally first
                if (!_jwtClientService.IsTokenValid(_currentToken))
                {
                    ClearUserToken();
                    return false;
                }

                // Optionally validate with API
                var isValid = await ValidateTokenAsync(_currentToken);
                
                if (!isValid)
                {
                    ClearUserToken();
                    return false;
                }

                AuthenticationStateChanged?.Invoke(true);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking authentication status");
                ClearUserToken();
                return false;
            }
        }
    }
}