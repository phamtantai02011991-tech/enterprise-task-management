namespace TaskManagementWeb.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int OverdueTasks { get; set; }
        public List<TaskViewModel> RecentTasks { get; set; } = new();
    }
}
