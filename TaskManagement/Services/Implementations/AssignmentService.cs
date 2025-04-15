using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;
namespace TaskManagement.Services.Implementations
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentService(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<bool> ToggleAssignmentAsync(Guid subTaskId, Guid userId)
        {
            return await _assignmentRepository.ToggleAssignmentAsync(subTaskId, userId);
        }

        public async Task<List<UserModel>> GetAssignedUsersAsync(Guid subTaskId)
        {
            return await _assignmentRepository.GetAssignedUsersAsync(subTaskId);
        }
    }
}
