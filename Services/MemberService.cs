using FootballerWeb.Models;
using FootballerWeb.DTOs;

namespace FootballerWeb.Services
{
    public interface IMemberService
    {
        Task<Member?> GetMemberByEmailAsync(string email);
        Task<Member?> GetMemberByProviderIdAsync(string providerId, string provider);
        Task<Member> RegisterMemberAsync(RegisterMemberDto registerDto);
        Task<Member> UpdateLastLoginAsync(int memberId);
        Task<bool> IsMemberExistsByEmailAsync(string email);
        Task<List<Member>> GetAllMembersAsync();
    }

    public class MemberService : IMemberService
    {
        private readonly ILogger<MemberService> _logger;
        // En un proyecto real, aquí inyectarías tu DbContext
        private static readonly List<Member> _members = new();

        public MemberService(ILogger<MemberService> logger)
        {
            _logger = logger;
        }

        public async Task<Member?> GetMemberByEmailAsync(string email)
        {
            await Task.CompletedTask; // Simular operación async
            return _members.FirstOrDefault(m => m.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && m.IsActive);
        }

        public async Task<Member?> GetMemberByProviderIdAsync(string providerId, string provider)
        {
            await Task.CompletedTask; // Simular operación async
            return _members.FirstOrDefault(m => m.ProviderId == providerId && m.Provider == provider && m.IsActive);
        }

        public async Task<Member> RegisterMemberAsync(RegisterMemberDto registerDto)
        {
            await Task.CompletedTask; // Simular operación async
            
            var member = new Member
            {
                Id = _members.Count + 1,
                Email = registerDto.Email,
                Name = registerDto.Name,
                ProfilePictureUrl = registerDto.ProfilePictureUrl,
                Provider = registerDto.Provider,
                ProviderId = registerDto.ProviderId,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsActive = true
            };

            _members.Add(member);
            _logger.LogInformation("New member registered: {Email} via {Provider}", member.Email, member.Provider);
            
            return member;
        }

        public async Task<Member> UpdateLastLoginAsync(int memberId)
        {
            await Task.CompletedTask; // Simular operación async
            
            var member = _members.FirstOrDefault(m => m.Id == memberId);
            if (member != null)
            {
                member.LastLoginAt = DateTime.UtcNow;
                _logger.LogInformation("Updated last login for member: {Email}", member.Email);
            }
            
            return member!;
        }

        public async Task<bool> IsMemberExistsByEmailAsync(string email)
        {
            await Task.CompletedTask; // Simular operación async
            return _members.Any(m => m.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && m.IsActive);
        }

        public async Task<List<Member>> GetAllMembersAsync()
        {
            await Task.CompletedTask; // Simular operación async
            return _members.Where(m => m.IsActive).ToList();
        }
    }
}