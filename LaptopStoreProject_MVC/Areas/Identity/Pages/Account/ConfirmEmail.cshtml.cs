// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LaptopStoreProject_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace LaptopStoreProject_MVC.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ConfirmEmailModel> _logger;

        public ConfirmEmailModel(UserManager<AppUser> userManager, ILogger<ConfirmEmailModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
           
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        //public async Task<IActionResult> OnGetAsync(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return RedirectToPage("/Index");
        //    }

        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return NotFound($"Unable to load user with ID '{userId}'.");
        //    }

        //    code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        //    var result = await _userManager.ConfirmEmailAsync(user, code);
        //    StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
        //    return Page();
        //}

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            _logger.LogInformation("Confirming email for user {UserId} with code {Code}", userId, code);
            // Kiểm tra nếu userId hoặc code null
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                StatusMessage = "Invalid email confirmation request.";
                return Page(); // Hiển thị lỗi thay vì chuyển hướng
            }

            // Tìm người dùng dựa trên userId
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            try
            {
                // Giải mã code từ Base64Url
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (Exception ex)
            {
                // Nếu giải mã thất bại
                StatusMessage = "Invalid email confirmation code.";
                return Page(); // Hiển thị lỗi thay vì chuyển hướng
            }

            // Xác nhận email
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Page();
            }
            else
            {
                StatusMessage = "Error confirming your email.";
            }

            return Page();
        }

    }
}
