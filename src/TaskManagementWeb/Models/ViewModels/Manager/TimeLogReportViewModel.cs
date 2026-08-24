using TaskManagementWeb.Models.Entities;

namespace TaskManagementWeb.ViewModels.Manager
{
    public class TimeLogReportViewModel
    {
        public int? ProjectId { get; set; }
        public string? ProjectTitle { get; set; }

        public List<TimeLog> TimeLogs { get; set; } = new();

        public double TotalHours => TimeLogs.Sum(tl => tl.HoursSpent);
        
        public int TotalLogsCount => TimeLogs.Count;
    }
}