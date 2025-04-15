using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<bool> ToggleAssignmentAsync(Guid subTaskId, Guid userId);
        Task<List<UserModel>> GetAssignedUsersAsync(Guid subTaskId);
    }
}
