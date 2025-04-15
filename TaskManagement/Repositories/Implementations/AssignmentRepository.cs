using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Repositories.Implementations
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly TaskListDbContext _context;

        public AssignmentRepository(TaskListDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleAssignmentAsync(Guid subTaskId, Guid userId)
        {
            var existing = await _context.SubTaskAssignments
                .FirstOrDefaultAsync(x => x.SubTaskId == subTaskId && x.UserId == userId);

            if (existing != null)
            {
                _context.SubTaskAssignments.Remove(existing);
                await _context.SaveChangesAsync();
                return false;
            }

            var assignment = new SubTaskAssignmentModel
            {
                SubTaskId = subTaskId,
                UserId = userId
            };

            _context.SubTaskAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserModel>> GetAssignedUsersAsync(Guid subTaskId)
        {
            return await _context.SubTaskAssignments
                .Where(x => x.SubTaskId == subTaskId)
                .Select(x => x.User)
                .ToListAsync();
        }
    }
}
