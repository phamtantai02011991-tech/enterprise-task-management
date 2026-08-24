using TaskManagementWeb.Models.Entities;
using TaskManagementWeb.Models.ViewModels.Admin;

namespace TaskManagementWeb.Services.Admin
{
    public interface IAdminUserService
    {
        Task<UserListViewModel> GetUsersAsync(string? searchKey, int? roleIdFilter, int? departmentIdFilter, int page = 1, int pageSize = 10);
        Task<User?> GetUserByIdAsync(int id);
        Task<EditUserViewModel?> GetEditUserViewModelAsync(int id);
        Task<CreateUserViewModel> PrepareCreateUserViewModelAsync();
        Task<(bool Success, string Message)> CreateUserAsync(CreateUserViewModel model, int actionByUserId, string actionByUserName);
        Task<(bool Success, string Message)> UpdateUserAsync(EditUserViewModel model, int actionByUserId, string actionByUserName);
        Task<(bool Success, string Message)> ResetPasswordAsync(int userId, string newPassword, int actionByUserId, string actionByUserName);
        Task<(bool Success, string Message)> ToggleUserStatusAsync(int userId, int actionByUserId, string actionByUserName);
        Task<(bool Success, string Message)> DeleteUserAsync(int userId, int actionByUserId, string actionByUserName);
        Task<List<Role>> GetRolesAsync();
        Task<List<Department>> GetDepartmentsAsync();
    }
}
