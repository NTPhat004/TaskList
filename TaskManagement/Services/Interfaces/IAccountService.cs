using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces
{
	public interface IAccountService
	{
        Task<UserModel> AuthenticateGoogleUserAsync();
        Task SignInUserAsync(UserModel user);
        Task<UserModel> AuthenticateWithGoogleAsync(string email, string googleId, string name, string picture);
        Guid GetCurrentUserId();

    }

}
