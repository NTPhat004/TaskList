using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;

namespace TaskManagement.Repositories.Implementations
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly TaskListDbContext _context;

        public ActivityLogRepository(TaskListDbContext context)
        {
            _context = context;
        }
        public async Task<List<ActivityLogModel>> GetLogsByUserAsync(Guid userId)
        {
            return await _context.ActivityLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.Timestamp)
                .Include(log => log.User)
                .ToListAsync();
        }

        public async Task<List<ActivityLogModel>> GetLogsBySourceAsync(Guid userId, ActivityLogModel.ActivitySourceType source)
        {
            return await _context.ActivityLogs
                .Where(log => log.UserId == userId && log.Source == source)
                .OrderByDescending(log => log.Timestamp)
                .Include(log => log.User)
                .ToListAsync();
        }

        public async Task AddAsync(ActivityLogModel log)
        {
            await _context.ActivityLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
