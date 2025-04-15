using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;

namespace TaskManagement.Repositories.Implementations
{
    public class GroupMemberRepositoy : IGroupMemberRepository
    {
        private readonly TaskListDbContext _context;

        public GroupMemberRepositoy(TaskListDbContext context)
        {
            _context = context;
        }
        public async Task AddMemberAsync(GroupMemberModel member)
        {        
            _context.GroupMembers.Add(member);
            await _context.SaveChangesAsync();
        }
        public async Task<List<GroupMemberModel>> GetGroupMembersByGroupIdAsync(Guid groupId)
        {
            return await _context.GroupMembers
                .Include(gm => gm.User)
                .Where(gm => gm.GroupId == groupId)
                .OrderBy(gm => gm.JoinedAt)
                .ToListAsync();
        }
        public async Task<List<UserModel>> GetMembersWithUserByGroupId(Guid groupId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Include(gm => gm.User)
                .Select(gm => gm.User)
                .ToListAsync();
        }
    }
}
