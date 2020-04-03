using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.App.Data;
using Web.App.Helpers;
using Web.App.Models.Account;
using Web.App.Models.Email;
using Web.App.Models.User;
using Web.App.Services;

namespace Web.App.Managers
{
    public class AccountManager : IAccountManager
    {
        #region Private members
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;
        public IUserService _userService;
        public IConfiguration _configuration;
        public ILogger<AccountManager> _logger;
        #endregion

        public AccountManager(ApplicationDbContext dbContext,
           IMapper mapper,
           SignInManager<ApplicationUser> signManager,
           UserManager<ApplicationUser> userManager,
           IUserService userService,
           IConfiguration configuration,
           ILogger<AccountManager> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _signInManager = signManager;
            _userManager = userManager;
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
        }


        #region Public methods
        public async Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel)
        {
            var user = new ApplicationUser()
            {
                UserName = userModel.Email,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                if (!string.IsNullOrEmpty(token))
                {
                    await SendConfirmationEmailLinkToUserAsync(user, token);
                }
            }
            return result;
        }

        public async Task SignInAsync(SignInModel model)
        {
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, SecurityStamp = new Guid().ToString() };
            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        public async Task<SignInResult> PasswordSignInAsync(SignInModel model, ApplicationUser user)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.StaySignIn, false);
            return result;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model)
        {
            var userId = _userService.GetUserId();
            return await _userManager.ChangePasswordAsync(await _userManager.FindByIdAsync(userId), model.CurrentPassword, model.NewPassword);
        }

        public async Task<bool> SendForgotPasswordEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (!string.IsNullOrEmpty(token))
                {
                    await SendForgotPasswordEmailToUserAsync(user, token);
                }

                return true;
            }

            return false;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model)
        {
            return await _userManager.ResetPasswordAsync(await _userManager.FindByIdAsync(model.UserId), model.Token, model.Password);
        }

        public async Task<bool> SendConfirmEmailTokenEmailAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendConfirmationEmailLinkToUserAsync(user, token);
                return true;
            }

            return false;
        }

        public async Task<IdentityResult> ConfirmUserEmailAsync(string userId, string token)
        {
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(userId), token);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<UserModel> GetCurrentUserAsync()
        {
            var userId = _userService.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);

                return _mapper.Map<UserModel>(user);
            }
            return null;
        }

        public async Task<IdentityResult> UpdateUserBasicInfoAsync(UserModel model)
        {
            var user = await _userManager.FindByIdAsync(_userService?.GetUserId());
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                return await _userManager.UpdateAsync(user);
            }

            return null;
        }

        public async Task<bool> IsEmailExist(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<bool> IsAdmin(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, "Admin");
        }
        #endregion

        #region Private methods

        private async Task SendForgotPasswordEmailToUserAsync(ApplicationUser user, string token)
        {
            EmailSender emailSender = new EmailSender(_configuration);

            string link = _configuration.GetSection("ApplicationContent").GetSection("AppDomain").Value
                + _configuration.GetSection("ApplicationContent").GetSection("ResetPasswordLink").Value;

            UserEmailOptions userEmailOptions = new UserEmailOptions()
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{FirstName}}", user.FirstName),
                    new KeyValuePair<string, string>("{{Link}}", string.Format(link, token, user.Id))
                }
            };

            await Task.Run(async () =>
            {
                await emailSender.SendForgotPasswordEmailToUserAsync(userEmailOptions);
            });
        }

        private async Task SendConfirmationEmailLinkToUserAsync(ApplicationUser user, string token)
        {
            EmailSender emailSender = new EmailSender(_configuration);

            string link = _configuration.GetSection("ApplicationContent").GetSection("AppDomain").Value
                + _configuration.GetSection("ApplicationContent").GetSection("ConfirmPasswordLink").Value;

            UserEmailOptions userEmailOptions = new UserEmailOptions()
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{FirstName}}", user.FirstName),
                    new KeyValuePair<string, string>("{{Link}}", string.Format(link, token, user.Id))
                }
            };

            await Task.Run(async () =>
            {
                await emailSender.SendConfirmationEmailLinkToUserAsync(userEmailOptions);
            });
        }

        #endregion
    }
}
