using Microsoft.EntityFrameworkCore;
using System;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
namespace TaskManagement.Repositories.Implementations
{
    public class GroupRepository : IGroupRepository
    {
        private readonly TaskListDbContext _context;

        public GroupRepository(TaskListDbContext context)
        {
            _context = context;
        }
        public async Task AddGroupAsync(GroupModel group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GroupModel>> GetUserGroupsAsync(Guid userId)
        {
            return await _context.Groups
             .Where(g => g.Members.Any(m => m.UserId == userId) || g.CreatedBy == userId)
             .ToListAsync();

        }
        public async Task<GroupModel?> GetByIdAsync(Guid groupId)
        {
            return await _context.Groups
               .Include(g => g.Members)                  // Include GroupMembers
                   .ThenInclude(gm => gm.User)          // Include User trong GroupMember
               .FirstOrDefaultAsync(g => g.Id == groupId);
        }
    }
}
