using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;

namespace TaskManagement.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskListDbContext _context;

        public TaskRepository(TaskListDbContext context)
        {
            _context = context;
        }

        //Lấy về danh sách Mục công việc dựa theo id của người dùng(Việc cá nhân)
        public async Task<List<TaskModel>> GetTasksByUserIdAsync(Guid userId)
        {
            return await _context.Tasks
           .Where(t => t.OwnerId == userId && !t.IsGroupTask)
           .Include(t => t.SubTasks) // Load luôn cả SubTasks
           .ToListAsync();  
        }

        //Lấy về danh sách Mục công việc dựa theo id của Nhóm(Công việc nhóm)
        public async Task<List<TaskModel>> GetTasksByGroupIdAsync(Guid groupId)
        {
            return await _context.Tasks
                        .Where(t => t.GroupId == groupId && t.IsGroupTask)
                        .Include(t => t.SubTasks)
                            .ThenInclude(st => st.Assignments)
                                .ThenInclude(a => a.User) // lấy luôn thông tin User trong mỗi Assignment
                        .OrderByDescending(t => t.CreatedAt)
                        .ToListAsync();
        }

        public async Task<TaskModel> GetTaskByIdAsync(Guid taskId)
        {
            return await _context.Tasks.FindAsync(taskId);
        }

        public async Task<TaskModel> CreateAsync(TaskModel task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteAsync(Guid taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTaskAsync(TaskModel task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SubTaskModel>> GetSubTasksByTaskIdAsync(Guid taskId)
        {
            return await _context.SubTasks.Where(s => s.TaskId == taskId).ToListAsync();
        }

        public async Task<SubTaskModel> GetSubTaskByIdAsync(Guid subTaskId)
        {
            return await _context.SubTasks
                    .Include(st => st.Assignments)
                        .ThenInclude(a => a.User)
                    .Include(ts => ts.Task)
                        .FirstOrDefaultAsync(t => t.Id == subTaskId);
        }

        public async Task<SubTaskModel> CreateAsync(SubTaskModel subTask)
        {
            _context.SubTasks.Add(subTask);
            await _context.SaveChangesAsync();
            return subTask;
        }

        public async Task UpdateAsync(SubTaskModel subTask)
        {
            _context.SubTasks.Update(subTask);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SubTaskModel>> GetTodaySubTasksAsync(Guid userId)
        {
            var today = DateTime.Today;
            return await _context.SubTasks
                .Include(st => st.Task)
                .Where(st =>
                    st.DueDate.HasValue &&
                    st.DueDate.Value.Date == today &&
                    (st.CreatedBy == userId /*|| st.AssignedTo == userId*/)
                )
                .ToListAsync();
        }

        public async Task<TaskModel> GetTaskWithSubTasksAndAssignmentsAsync(Guid taskId)
        {
            return await _context.Tasks
                    .Include(t => t.SubTasks)
                        .ThenInclude(st => st.Assignments)
                            .ThenInclude(a => a.User)
                    .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task<SubTaskModel> GetByIdWithAssignmentsAsync(Guid subTaskId)
        {
            return await _context.SubTasks
                .Include(st => st.Assignments)
                    .ThenInclude(a => a.User)
                .Include(st => st.Task)
                    .ThenInclude(t => t.Group)
                .FirstOrDefaultAsync(st => st.Id == subTaskId);
        }

    }

}
