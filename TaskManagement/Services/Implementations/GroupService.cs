using Microsoft.EntityFrameworkCore;
using TaskManagement.Common;
using TaskManagement.Models;
using TaskManagement.Repositories.Implementations;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ITaskRepository _taskRepository;

        public GroupService(IGroupRepository groupRepository, ITaskRepository taskRepository)
        {
            _groupRepository = groupRepository;
            _taskRepository = taskRepository;
        }

        public async Task<ServiceResult<List<GroupModel>>> GetUserGroupsAsync(Guid userId)
        {
            var groups = await _groupRepository.GetUserGroupsAsync(userId);

            if (groups == null || !groups.Any())
            {
                return ServiceResult<List<GroupModel>>.NotFound("Người dùng chưa tham gia nhóm nào.");
            }

            return ServiceResult<List<GroupModel>>.Success(groups);
        }

        public async Task<ServiceResult<List<GroupModel>>> CreateGroupAsync(Guid userId, string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return ServiceResult<List<GroupModel>>.ValidationError(new List<string> { "Tên nhóm không hợp lệ!" });
            }

            var newGroup = new GroupModel
            {
                Id = Guid.NewGuid(),
                Name = groupName,
                CreatedBy = userId,
            };

            await _groupRepository.AddGroupAsync(newGroup);

            var groups = await _groupRepository.GetUserGroupsAsync(userId);

            return ServiceResult<List<GroupModel>>.Success(groups);
        }

        public async Task<GroupModel?> GetGroupByIdAsync(Guid groupId)
        {
            return await _groupRepository.GetByIdAsync(groupId);
        }

        public async Task<List<TaskModel>> GetTasksByGroupIdAsync(Guid groupId)
        {
            return await _taskRepository.GetTasksByGroupIdAsync(groupId);
        }
    }
}
