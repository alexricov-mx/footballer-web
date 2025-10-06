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
}