using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.ViewModels.Admin;

namespace TaskManagementWeb.Services.Admin
{
    public interface IDepartmentService
    {
        Task<DepartmentListViewModel> GetDepartmentsAsync(string? searchKey);
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<DepartmentFormViewModel?> GetDepartmentFormViewModelAsync(int id);
        Task<(bool Success, string Message)> CreateDepartmentAsync(DepartmentFormViewModel model, int actionByUserId, string actionByUserName);
        Task<(bool Success, string Message)> UpdateDepartmentAsync(DepartmentFormViewModel model, int actionByUserId, string actionByUserName);
        Task<(bool Success, string Message)> DeleteDepartmentAsync(int id, int actionByUserId, string actionByUserName);
    }
}
