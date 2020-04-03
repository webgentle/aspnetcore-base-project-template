using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Web.App.Managers;
using Web.App.Models.Account;

namespace Web.App.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountManager _accountManager;

        public AccountController(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        [AllowAnonymous, Route("login")]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            var model = new SignInModel
            {
                ReturnUrl = returnUrl
            };
            return await Task.Run(() => View(model));
        }

        [HttpPost, AllowAnonymous, Route("login")]
        public async Task<IActionResult> Login(SignInModel userModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountManager.GetUserByEmailAsync(userModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No account found. Please signup");
                    return View();
                }

                var result = await _accountManager.PasswordSignInAsync(userModel, user);
                if (result.Succeeded)
                {
                    if (!user.EmailConfirmed)
                    {
                        return RedirectToAction("ConfirmEmail", "Account", new { email = userModel.Email });
                    }

                    if (!string.IsNullOrEmpty(userModel.ReturnUrl))
                    {
                        return LocalRedirect(userModel.ReturnUrl);
                    }

                    if (await _accountManager.IsAdmin(user))
                    {
                        return RedirectToAction("Index", "Home", new { Area = "Admin" });
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Invalid credentials");
            return View();
        }

        [AllowAnonymous, Route("signup")]
        public async Task<IActionResult> SignUp()
        {
            return await Task.Run(() => View());
        }

        [HttpPost, AllowAnonymous, Route("signup")]
        public async Task<IActionResult> SignUp(SignUpUserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountManager.CreateUserAsync(userModel);
                if (!result.Succeeded)
                {
                    foreach (var errorMessage in result.Errors)
                    {
                        if (errorMessage.Code != "DuplicateUserName")
                        {
                            ModelState.AddModelError("", errorMessage.Description);
                        }
                    }

                    return View(userModel);
                }

                ModelState.Clear();

                return RedirectToAction("ConfirmEmail", "Account", new { email = userModel.Email });
            }
            return View();
        }

        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("change-password")]
        public async Task<IActionResult> ChangePassword()
        {
            return await Task.Run(() => View(new ChangePasswordModel()));
        }

        [HttpPost, Route("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountManager.ChangePasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                    return View(model);
                }

                if (result.Errors?.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }

        [AllowAnonymous, Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword()
        {
            return await Task.Run(() => View());
        }

        [AllowAnonymous, HttpPost, Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                await _accountManager.SendForgotPasswordEmailAsync(model.Email);

                ModelState.Clear();
                model.IsSuccess = true;
                return View(model);
            }
            return View(model);
        }

        [AllowAnonymous, Route("reset-password")]
        public async Task<IActionResult> ResetPassword(string uid, string token)
        {
            ResetPasswordModel model = new ResetPasswordModel()
            {
                Token = token,
                UserId = uid
            };
            return await Task.Run(() => View(model));
        }

        [AllowAnonymous, HttpPost, Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.Token = model.Token.Replace(' ', '+');
                var result = await _accountManager.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                    return View(model);
                }
                else if (result?.Errors?.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return await Task.Run(() => View(model));
        }

        [AllowAnonymous, Route("confirm-email"), HttpGet]
        public async Task<IActionResult> ConfirmEmail(string uid = null,
            string token = null,
            string email = null)
        {
            ConfirmEmailModel model = new ConfirmEmailModel();
            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
                var result = await _accountManager.ConfirmUserEmailAsync(uid, token);
                if (result.Succeeded)
                {
                    model.IsConfirmed = true;
                    return View(model);
                }
            }

            model.Email = email;
            return View(model);
        }

        [AllowAnonymous, Route("confirm-email"), HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailModel model)
        {
            var user = await _accountManager.GetUserByEmailAsync(model.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    model.IsConfirmed = true;
                    return View(model);
                }

                var result = await _accountManager.SendConfirmEmailTokenEmailAsync(user);
                if (result)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                }
            }
            else
            {
                ModelState.AddModelError("", "There is some issue in sending confirmation link. Please contact us.");
            }

            return View(model);
        }

        [AcceptVerbs("GET", "POST"), AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            if (!await _accountManager.IsEmailExist(email))
            {
                return Json($"Email {email} is already in use.");
            }

            return Json(true);
        }
    }
}