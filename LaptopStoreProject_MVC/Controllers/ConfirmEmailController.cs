using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LaptopStoreProject_MVC.Controllers
{
    public class ConfirmEmailController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        // Tiêm UserManager qua constructor
        public ConfirmEmailController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home"); // hoặc trang bạn muốn nếu thiếu thông tin
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Home"); // hoặc trang bạn muốn nếu người dùng không tồn tại
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                // Chuyển hướng đến trang ConfirmEmail
                return RedirectToAction("ConfirmEmail");
            }
            else
            {
                // Nếu xác nhận thất bại, có thể thông báo lỗi cho người dùng
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
