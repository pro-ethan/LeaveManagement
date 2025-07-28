using LeaveManagement.Models.ViewModel;

namespace LeaveManagement.AuthImplementation.Abstract
{
    public interface IUserAuthService
    {
        Task<Status> LoginAsync(LoginViewModel model);
        Task<Status> RegisterAsync(RegisterViewModel model);

        Task LogoutAsync();
    }
}
