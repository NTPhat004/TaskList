using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Services.Implementations
{
	public class AccountService : IAccountService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

		public AccountService(IHttpContextAccessor httpContextAccessor,IUserRepository userRepository)
		{
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
		}

        public async Task<UserModel> AuthenticateGoogleUserAsync()
        {
            var authResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authResult.Succeeded) throw new Exception("Google authentication failed.");

            var claims = authResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var picture = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

            if (email == null || googleId == null)
            {
                throw new Exception("Google authentication data is missing.");
            }

            return await AuthenticateWithGoogleAsync(email, googleId, name, picture);
        }

        public async Task SignInUserAsync(UserModel user)
        {
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("GoogleId", user.GoogleId ?? ""),
                new Claim("ProfilePicture", user.ProfilePicture ?? ""),
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }

        public async Task<UserModel> AuthenticateWithGoogleAsync(string email, string googleId, string name, string picture)
		{
			var user = await _userRepository.GetUserByGoogleIdAsync(googleId);

			if (user == null)
			{
				user = new UserModel
				{
					Email = email,
					GoogleId = googleId,
					Username = name,
					ProfilePicture = picture,
					AuthProvider = "Google",
					CreatedAt = DateTime.UtcNow
				};

				await _userRepository.AddUserAsync(user);
				await _userRepository.SaveChangesAsync();
			}

			return user;
		}


        public Guid GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) throw new UnauthorizedAccessException("User is not authenticated");

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) throw new UnauthorizedAccessException("User ID is missing");

            return Guid.Parse(userIdClaim);
        }
    }
}
