using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces
{
    public interface IGroupMemberRepository
    {
       Task AddMemberAsync(GroupMemberModel member);
    }
}
