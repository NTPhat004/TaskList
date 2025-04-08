using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<UserModel?> GetUserByEmailAsync(string email);
		Task<UserModel?> GetUserByGoogleIdAsync(string googleId);
		Task AddUserAsync(UserModel user);
		Task SaveChangesAsync();
	}
}
