using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FootballerWeb.Services
{
    public interface IJwtClientService
    {
        bool IsTokenValid(string token);
        ClaimsPrincipal? GetClaimsFromToken(string token);
        bool IsTokenExpired(string token);
        string? GetUserEmailFromToken(string token);
        string? GetUserNameFromToken(string token);
        Dictionary<string, string> GetAllClaimsFromToken(string token);
        DateTime? GetTokenExpirationDate(string token);
        string? GetTokenIssuer(string token);
        string? GetTokenAudience(string token);
    }

    public class JwtClientService : IJwtClientService
    {
        private readonly ILogger<JwtClientService> _logger;

        public JwtClientService(ILogger<JwtClientService> logger)
        {
            _logger = logger;
        }

        public bool IsTokenValid(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return false;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                
                // Check if token is expired
                return !IsTokenExpired(token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error validating JWT token");
                return false;
            }
        }

        public ClaimsPrincipal? GetClaimsFromToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                
                var identity = new ClaimsIdentity(jsonToken.Claims, "jwt");
                return new ClaimsPrincipal(identity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting claims from JWT token");
                return null;
            }
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return true;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                
                return jsonToken.ValidTo < DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error checking token expiration");
                return true;
            }
        }

        public string? GetUserEmailFromToken(string token)
        {
            try
            {
                var claims = GetClaimsFromToken(token);
                // Try multiple email claim types
                return claims?.FindFirst(ClaimTypes.Email)?.Value ??
                       claims?.FindFirst("email")?.Value ??
                       claims?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting email from token");
                return null;
            }
        }

        public string? GetUserNameFromToken(string token)
        {
            try
            {
                var claims = GetClaimsFromToken(token);
                // Try multiple name claim types
                return claims?.FindFirst(ClaimTypes.Name)?.Value ??
                       claims?.FindFirst("name")?.Value ??
                       claims?.FindFirst("given_name")?.Value ??
                       claims?.FindFirst("unique_name")?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting name from token");
                return null;
            }
        }

        public Dictionary<string, string> GetAllClaimsFromToken(string token)
        {
            var claimsDict = new Dictionary<string, string>();
            
            try
            {
                if (string.IsNullOrEmpty(token))
                    return claimsDict;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                
                foreach (var claim in jsonToken.Claims)
                {
                    // Use friendly names for common claims
                    var claimType = GetFriendlyClaimName(claim.Type);
                    
                    if (!claimsDict.ContainsKey(claimType))
                    {
                        claimsDict[claimType] = claim.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting all claims from token");
            }
            
            return claimsDict;
        }

        public DateTime? GetTokenExpirationDate(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                
                return jsonToken.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting token expiration date");
                return null;
            }
        }

        public string? GetTokenIssuer(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                
                return jsonToken.Issuer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting token issuer");
                return null;
            }
        }

        public string? GetTokenAudience(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                
                return jsonToken.Audiences?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting token audience");
                return null;
            }
        }

        private string GetFriendlyClaimName(string claimType)
        {
            return claimType switch
            {
                ClaimTypes.Name => "Nombre",
                ClaimTypes.Email => "Email",
                ClaimTypes.NameIdentifier => "ID de Usuario",
                ClaimTypes.GivenName => "Nombre",
                ClaimTypes.Surname => "Apellido",
                ClaimTypes.Role => "Rol",
                "sub" => "Subject (ID)",
                "iss" => "Emisor",
                "aud" => "Audiencia",
                "exp" => "Expiración",
                "iat" => "Emitido en",
                "nbf" => "No válido antes de",
                "jti" => "ID del Token",
                "picture" => "Foto de Perfil",
                "given_name" => "Nombre",
                "family_name" => "Apellido",
                "locale" => "Idioma/Región",
                "email_verified" => "Email Verificado",
                _ => claimType
            };
        }
    }
}