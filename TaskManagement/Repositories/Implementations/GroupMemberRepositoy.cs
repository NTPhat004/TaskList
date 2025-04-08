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
    }
}
