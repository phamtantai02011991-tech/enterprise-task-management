namespace TaskManagementWeb.Models.Enums
{
    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }


    public enum TaskStatusEnum
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }


    public enum UserRoleEnum
    {
        Admin = 1,
        Manager = 2,
        Employee = 3
    }
}