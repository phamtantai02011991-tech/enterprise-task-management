using Microsoft.EntityFrameworkCore;
using TaskManagementWeb.Data;
using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.ViewModels.Manager;
using TaskManagementWeb.Models.Enums;

namespace TaskManagementWeb.Services.Manager
{
    public class TaskManagementService : ITaskManagementService
    {
        private readonly ApplicationDbContext _context;

        public TaskManagementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GetTasksByProjectIdAsync(int projectId)
        {
            return await _context.TaskItems
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.AssignedUser)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.AssignedUser)
                .Include(t => t.TimeLogs)
                    .ThenInclude(tl => tl.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CreateTaskAsync(TaskCreateUpdateViewModel model)
        {
            var taskItem = new TaskItem
            {
                Title = model.Title,
                Description = model.Description,
                Priority = Enum.Parse<TaskPriority>(model.Priority), // Hoặc gán trực tiếp tùy kiểu Enum của bạn
                Status = Enum.Parse<TaskStatusEnum>(model.Status),
                Deadline = model.Deadline,
                ProjectId = model.ProjectId,
                AssignedUserId = model.AssignedUserId
            };

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(TaskCreateUpdateViewModel model)
        {
            var taskItem = await _context.TaskItems.FindAsync(model.Id);
            if (taskItem != null)
            {
                taskItem.Title = model.Title;
                taskItem.Description = model.Description;
                taskItem.Priority = Enum.Parse<TaskPriority>(model.Priority);
                taskItem.Status = Enum.Parse<TaskStatusEnum>(model.Status);
                taskItem.Deadline = model.Deadline;
                taskItem.AssignedUserId = model.AssignedUserId;

                _context.TaskItems.Update(taskItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTaskAsync(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem != null)
            {
                // Xóa mềm: Chuyển sang Cancelled và IsActive = false, không xóa khỏi DB để giữ lịch sử TimeLog
                taskItem.IsActive = false;
                taskItem.Status = TaskStatusEnum.Cancelled;
                _context.TaskItems.Update(taskItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}