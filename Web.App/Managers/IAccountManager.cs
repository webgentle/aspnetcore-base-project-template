using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Web.App.Models.Account;
using Web.App.Models.User;

namespace Web.App.Managers
{
    public interface IAccountManager
    {
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model);
        Task<IdentityResult> ConfirmUserEmailAsync(string userId, string token);
        Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel);
        Task<UserModel> GetCurrentUserAsync();
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<bool> IsAdmin(ApplicationUser user);
        Task<bool> IsEmailExist(string email);
        Task<SignInResult> PasswordSignInAsync(SignInModel model, ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);
        Task<bool> SendConfirmEmailTokenEmailAsync(ApplicationUser user);
        Task<bool> SendForgotPasswordEmailAsync(string email);
        Task SignInAsync(SignInModel model);
        Task SignOutAsync();
        Task<IdentityResult> UpdateUserBasicInfoAsync(UserModel model);
    }
}