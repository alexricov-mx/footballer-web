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

    // DTOs para Ligas e Invitaciones de Liga
    public class Liga
    {
        public int IdLiga { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; } = string.Empty; // abierta, cerrada, iniciada, finalizada
        public int CantidadEquipos { get; set; }
        public string CreadorNombre { get; set; } = string.Empty;
        public string MiRolEnLiga { get; set; } = string.Empty; // admin_liga, jugador, etc.
        public bool PuedoInvitar { get; set; }
    }

    public class InvitacionLiga
    {
        public int IdInvitacion => IdInvitacionLiga; // Propiedad calculada para compatibilidad
        public int IdInvitacionLiga { get; set; } // ID principal del backend
        public string CodigoInvitacion { get; set; } = string.Empty;
        public int IdLiga { get; set; }
        public string NombreLiga { get; set; } = string.Empty;
        public string EmailInvitado { get; set; } = string.Empty;
        public string? NombreInvitador { get; set; } = string.Empty;
        public int InvitadoPor { get; set; } // ID del usuario que invitó
        public string TipoInvitacion { get; set; } = string.Empty;
        public DateTime FechaInvitacion { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public string Estado => Usado ? "aceptada" : "pendiente"; // Propiedad calculada basada en 'Usado'
        public bool Usado { get; set; } = false; // Campo del backend
        public string? MensajePersonalizado { get; set; }
        public bool Vigente { get; set; }
    }

    public class InvitacionLigaResponse
    {
        public int IdInvitacionLiga { get; set; }
        public string CodigoInvitacion { get; set; } = string.Empty;
        public int IdLiga { get; set; }
        public string NombreLiga { get; set; } = string.Empty;
        public string EmailInvitado { get; set; } = string.Empty;
        public string TipoInvitacion { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public string UrlUnirse { get; set; } = string.Empty;
        public string? MensajePersonalizado { get; set; }
        
        // Para compatibilidad con el código existente
        public int IdInvitacion => IdInvitacionLiga;
    }

    // DTO para crear liga
    public class CrearLigaRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Estatus { get; set; } = "abierta";
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    // DTOs para Equipos
    public class Equipo
    {
        public int IdEquipo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? IdLiga { get; set; }
        // NOTA: IdCuentaUsuario removido - los equipos son independientes
        // La relación usuario-equipo se maneja a través de liga_equipo
        public bool Vigente { get; set; } = true;
        public bool Seleccionado { get; set; } = false; // Para UI
    }

    // DTO para torneos donde el usuario participa
    public class TorneoParticipacion
    {
        public int IdLiga { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = string.Empty; // "proximo", "activo", "finalizado"
        public int TotalEquipos { get; set; }
        
        // Información del equipo del usuario
        public string? MiEquipo { get; set; }
        public int? MiEquipoId { get; set; }
        
        // Rol del usuario en el torneo
        public string MiRolEnTorneo { get; set; } = string.Empty; // "admin_liga", "jugador", etc.
        
        // Estadísticas del usuario/equipo en el torneo
        public int PartidosJugados { get; set; }
        public int Posicion { get; set; }
        public int Puntos { get; set; }
        public int Ganados { get; set; }
        public int Empatados { get; set; }
        public int Perdidos { get; set; }
        public int GolesFavor { get; set; }
        public int GolesContra { get; set; }
        public int DiferenciaGoles => GolesFavor - GolesContra;
        
        // Información adicional
        public string? ProximoPartido { get; set; }
        public DateTime? FechaProximoPartido { get; set; }
        public string? UltimoResultado { get; set; }
        public DateTime? FechaUltimoPartido { get; set; }
    }
}