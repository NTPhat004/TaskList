using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;
using TaskManagement.Repositories.Implementations;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IActivityLogService _activityLogService;

        public TaskService(ITaskRepository taskRepository,IActivityLogService activityLogService)
        {
            _taskRepository = taskRepository;
            _activityLogService = activityLogService;
        }

        //Lấy danh sách Mục công việc của người dùng(cá nhân)
        public async Task<List<TaskModel>> GetTasksByUserIdAsync(Guid userId)
        {
            return await _taskRepository.GetTasksByUserIdAsync(userId);
        }

        //Lấy Mục công việc dựa vào id 
        public async Task<TaskModel> GetTaskByIdAsync(Guid taskId)
        {
            return await _taskRepository.GetTaskByIdAsync(taskId);
        }

        //Tạo  Mục công việc mới
        public async Task<TaskModel> CreateTaskAsync(TaskModel task)
        {
            return await _taskRepository.CreateAsync(task);
        }

        //Xóa Mục công việc
        public async Task DeleteTaskAsync(Guid taskId)
        {
            await _taskRepository.DeleteAsync(taskId);
        }

        //Thay đổi thông tin Mục công việc
        public async Task UpdateTaskAsync(TaskModel task)
        {
            await _taskRepository.UpdateTaskAsync(task);
        }

        //Lấy danh sách Công việc theo id của Mục
        public async Task<List<SubTaskModel>> GetSubTasksByTaskIdAsync(Guid taskId)
        {
            return await _taskRepository.GetSubTasksByTaskIdAsync(taskId);
        }

        //Lấy công việc bằng id
        public async Task<SubTaskModel> GetSubTaskByIdAsync(Guid subTaskId)
        {
            return await _taskRepository.GetSubTaskByIdAsync(subTaskId);
        }

        //Tạo công việc mới
        public async Task<SubTaskModel> CreateSubTaskAsync(SubTaskModel subTask)
        {
            return await _taskRepository.CreateAsync(subTask);
        }

        //Thay đổi thông tin công việc
        public async Task UpdateSubTaskAsync(SubTaskModel subTask)
        {
            await _taskRepository.UpdateAsync(subTask);
        }

        //Thay đổi trạng thái hoàn thành / bỏ hoàn thành và ghi vào hoạt động
        public async Task<SubTaskModel?> ToggleSubTaskStatusAsync(Guid subTaskId,Guid userId)
        {
            var subTask = await _taskRepository.GetSubTaskByIdAsync(subTaskId);
            if (subTask == null) return null;

            subTask.IsCompleted = !subTask.IsCompleted;
            await _taskRepository.UpdateAsync(subTask);


            await _activityLogService.LogSubTaskStatusChangeAsync(subTask,userId);

            return subTask;
        }

        //Lấy về những công việc sẽ đến hạn trong ngày
        public async Task<List<SubTaskModel>> GetTodaySubTasksAsync(Guid userId)
        {
            return await _taskRepository.GetTodaySubTasksAsync(userId);
        }

        //Lấy danh sách Mục công việc theo id Nhóm
        public async Task<List<TaskModel>> GetGroupTasksAsync(Guid groupId)
        {
            return await _taskRepository.GetTasksByGroupIdAsync(groupId);
        }

        //Lấy danh sách Công việc và cả phân công
        public async Task<TaskModel> GetTaskWithSubTasksAndAssignments(Guid taskId)
        {
            return await _taskRepository.GetTaskWithSubTasksAndAssignmentsAsync(taskId);
        }

        public async Task<SubTaskModel> GetSubTaskDetailAsync(Guid subTaskId)
        {
            return await _taskRepository.GetByIdWithAssignmentsAsync(subTaskId);
        }

    }

}
