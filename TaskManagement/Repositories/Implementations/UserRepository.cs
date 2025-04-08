using TaskManagement.Data;
using TaskManagement.Models;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Repositories.Interfaces;

namespace TaskManagement.Repositories.Implementations
{
	public class UserRepository : IUserRepository
	{
		private readonly TaskListDbContext _context;

		public UserRepository(TaskListDbContext context)
		{
			_context = context;
		}

		public async Task<UserModel?> GetUserByEmailAsync(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		public async Task<UserModel?> GetUserByGoogleIdAsync(string googleId)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
		}

		public async Task AddUserAsync(UserModel user)
		{
			await _context.Users.AddAsync(user);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}

}
