namespace FootballerWeb.DTOs
{
    public class LoginRequestDto
    {
        public string Provider { get; set; } = string.Empty; // "Microsoft" o "Google"
        public string AuthorizationCode { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public MemberDto? Member { get; set; }
        public string? AccessToken { get; set; }
        public string? JwtToken { get; set; }
        public bool IsNewMember { get; set; }
    }

    public class MemberDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string Provider { get; set; } = string.Empty;
        public DateTime LastLoginAt { get; set; }
    }

    public class RegisterMemberDto
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ProviderId { get; set; } = string.Empty;
    }

    public class OAuthUserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string Provider { get; set; } = string.Empty;
    }

    // DTOs para gestión de usuarios
    public class UsuarioDashboard
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RolNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int TotalLigas { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public string? FotoPerfil { get; set; }
    }

    public class DetalleUsuario
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RolNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public string? FotoPerfil { get; set; }
        public List<LigaParticipacion> Ligas { get; set; } = new();
    }

    public class LigaParticipacion
    {
        public int IdLiga { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string RolEnLiga { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
        public bool Activa { get; set; }
    }

    public class InvitacionResponse
    {
        public int IdInvitacion { get; set; }
        public string CodigoInvitacion { get; set; } = string.Empty;
        public string EmailInvitado { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public string UrlRegistro { get; set; } = string.Empty;
        public string? MensajePersonalizado { get; set; }
    }

    public class RolUsuario
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class InvitacionPendiente
    {
        public int IdInvitacion { get; set; }
        public string CodigoInvitacion { get; set; } = string.Empty;
        public string EmailInvitado { get; set; } = string.Empty;
        public int InvitadoPor { get; set; }
        public string NombreInvitador { get; set; } = string.Empty;
        public DateTime FechaInvitacion { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public string? MensajePersonalizado { get; set; }
        public bool Usado { get; set; }
        public bool Vigente { get; set; }
    }
}