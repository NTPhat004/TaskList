using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task<bool> ToggleAssignmentAsync(Guid subTaskId, Guid userId);
        Task<List<UserModel>> GetAssignedUsersAsync(Guid subTaskId);
    }
}
