// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using LaptopStoreProject_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace LaptopStoreProject_MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ProviderDisplayName { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
        
        public IActionResult OnGet() => RedirectToPage("./Login");

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {

            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Lỗi từ nhà cung cấp bên ngoài: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Có lỗi khi tải thông tin đăng nhập bên ngoài.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            //bắt đầy từ đây

            // Lấy email từ thông tin đăng nhập bên ngoài
            //var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            //if (email == null)
            //{
            //    ErrorMessage = "Không thể lấy email từ nhà cung cấp bên ngoài.";
            //    return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            //}

            //// Kiểm tra xem email có tồn tại trong hệ thống chưa
            //var existingUser = await _userManager.FindByEmailAsync(email);
            //if (existingUser == null)
            //{
            //    // Điều hướng đến trang xác thực email
            //    TempData["ExternalLoginProvider"] = info.LoginProvider;
            //    TempData["Email"] = email;
            //    return RedirectToPage("./RegisterConfirm", new { ReturnUrl = returnUrl });
            //}

            //kết thúc

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        //public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        //{
        //    returnUrl = returnUrl ?? Url.Content("~/");

        //    if (remoteError != null)
        //    {
        //        ErrorMessage = $"Lỗi từ nhà cung cấp bên ngoài: {remoteError}";
        //        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        //    }

        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        ErrorMessage = "Có lỗi khi tải thông tin đăng nhập bên ngoài.";
        //        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        //    }

        //    // Lấy email từ thông tin đăng nhập bên ngoài
        //    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //    if (email == null)
        //    {
        //        ErrorMessage = "Không thể lấy email từ nhà cung cấp bên ngoài.";
        //        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        //    }

        //    // Kiểm tra xem email có tồn tại trong hệ thống chưa
        //    var existingUser = await _userManager.FindByEmailAsync(email);
        //    if (existingUser == null)
        //    {
        //        // Nếu người dùng chưa tồn tại trong hệ thống, điều hướng đến trang xác thực email
        //        TempData["ExternalLoginProvider"] = info.LoginProvider;
        //        TempData["Email"] = email;
        //        return RedirectToPage("./RegisterConfirmation", new { ReturnUrl = returnUrl });
        //    }

        //    // Nếu người dùng đã tồn tại, kiểm tra xem họ có đăng nhập qua provider bên ngoài này chưa
        //    var logins = await _userManager.GetLoginsAsync(existingUser);
        //    var isGmailLinked = logins.Any(login => login.LoginProvider == "Google"); // Tên provider là "Google"
        //    if (!isGmailLinked)
        //    {
        //        // Nếu email đã tồn tại nhưng không đăng nhập qua Gmail, chuyển hướng lại trang đăng nhập với provider
        //        return Redirect(Url.Action("Callback", "ExternalLogin", new { returnUrl }));
        //    }

        //    // Nếu email đã tồn tại và liên kết với provider bên ngoài (Gmail), thực hiện đăng nhập
        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        //    if (result.Succeeded)
        //    {
        //        _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
        //        return LocalRedirect(returnUrl);
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        return RedirectToPage("./Lockout");
        //    }
        //    else
        //    {
        //        // Nếu người dùng không có tài khoản, yêu cầu người dùng tạo tài khoản
        //        ReturnUrl = returnUrl;
        //        ProviderDisplayName = info.ProviderDisplayName;
        //        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
        //        {
        //            Input = new InputModel
        //            {
        //                Email = info.Principal.FindFirstValue(ClaimTypes.Email)
        //            };
        //        }
        //        return Page();
        //    }
        //}


        //asp-page-handler = "Confirmation" vs method = "post"
        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Lỗi khi tải thông tin đăng nhập bên ngoài trong quá trình xác nhận.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                //var registeredEmail = _userManager.FindByEmailAsync(Input.Email);
                //string externalEmail = null;
                //if(info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                //{
                //    externalEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
                //}

                var user = CreateUser();

                // Gán các giá trị cho user vừa tạo
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                //CreateAsync + đki dịch vụ RequireUniqueEmail = true; -> Kiểm tra Email là duy nhất
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }

        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}
