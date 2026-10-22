using Final.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final.Controllers
{
    public class AccountController : Controller
    {
        private const string UserIdSessionKey = "UserID";
        private const string UserNameSessionKey = "UserName";
        private readonly CmsContext _context;

        public AccountController(CmsContext context)
        {
            _context = context;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32(UserIdSessionKey).HasValue)
            {
                return RedirectToLocal(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password, string? returnUrl = null)
        {
            email = email?.Trim() ?? string.Empty;

            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.ReturnUrl = returnUrl;
                ModelState.AddModelError(string.Empty, "帳號或密碼錯誤。");
                return View();
            }

            HttpContext.Session.SetInt32(UserIdSessionKey, user.UserID);
            HttpContext.Session.SetString(UserNameSessionKey, user.Name);

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
