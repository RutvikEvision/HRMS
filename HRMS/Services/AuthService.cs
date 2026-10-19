using HRMS.Common;
using HRMS.Data.Entities;
using HRMS.DTOs.Auth;
using HRMS.Repositories.Interfaces;
using HRMS.Services.Interfaces;
using HRMS.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace HRMS.Services
{
    public class AuthService : IAuthService
    {
        private const string Provider = "HRMS";
        private const string TokenName = "RefreshToken";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMembershipRepository _memberships;
        private readonly JwtSettings _settings;

        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService, IMembershipRepository memberships, IOptions<JwtSettings> settings)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _memberships = memberships;
            _settings = settings.Value;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim();

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new AppException(400, string.Join(" ", result.Errors.Select(e => e.Description)));

            return new RegisterResponse { UserId = user.Id, Email = email };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim());

            if (user is null || !user.IsActive || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new AppException(401, "Invalid email or password.");

            return await IssueTokensAsync(user, null);
        }

        public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
        {
            var invalid = new AppException(401, "Invalid or expired refresh token.");

            var parts = request.RefreshToken.Split('.', 2);
            if (parts.Length != 2 || !Guid.TryParse(parts[0], out var userId)) throw invalid;

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null || !user.IsActive) throw invalid;

            var stored = (await _userManager.GetAuthenticationTokenAsync(user, Provider, TokenName))?.Split('|');

            if (stored is not { Length: 3 }
                || stored[0] != Hash(parts[1])
                || !long.TryParse(stored[1], out var ticks)
                || new DateTime(ticks, DateTimeKind.Utc) < DateTime.UtcNow)
                throw invalid;

            Guid? organizationId = Guid.TryParse(stored[2], out var id) ? id : null;
            return await IssueTokensAsync(user, organizationId);
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is not null)
                await _userManager.RemoveAuthenticationTokenAsync(user, Provider, TokenName);
        }

        public async Task<AuthResponse> IssueTokensAsync(ApplicationUser user, Guid? organizationId)
        {
            var memberships = await _memberships.GetActiveMembershipsAsync(user.Id);

            var active = memberships.FirstOrDefault(m => m.OrganizationId == organizationId)
                         ?? (memberships.Count == 1 ? memberships[0] : null);

            var (accessToken, expiresAt) = _tokenService.CreateAccessToken(
                user, active?.OrganizationId, active?.Role.Name);

            var secret = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(64));
            var refreshExpires = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays);

            await _userManager.SetAuthenticationTokenAsync(user, Provider, TokenName,
                $"{Hash(secret)}|{refreshExpires.Ticks}|{active?.OrganizationId}");

            return new AuthResponse
            {
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAt,
                RefreshToken = $"{user.Id:N}.{secret}"
            };
        }

        private static string Hash(string value) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }
}
