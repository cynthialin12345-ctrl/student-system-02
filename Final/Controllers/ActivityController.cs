using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final.Models;
using Final.Data;
using System;
using System.Linq;

namespace Final.Controllers
{
    public class ActivityController : Controller
    {
        private readonly CmsContext _context;
        private const string UserIdSessionKey = "UserID";

        public ActivityController(CmsContext context)
        {
            _context = context;
        }

        public IActionResult History(DateTime? date)
        {
            int? currentUserId = HttpContext.Session.GetInt32(UserIdSessionKey);
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(History), "Activity") });
            }

            var selectedDate = (date ?? DateTime.Today).Date;
            ViewBag.SelectedDate = selectedDate;

            var activities = _context.Activity
                .AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.Subcategory)
                .Where(a => a.UserID == currentUserId.Value && a.StartTime >= selectedDate && a.StartTime < selectedDate.AddDays(1))
                .Select(a => new ActivityViewModel
                {
                    UserName = a.User != null ? a.User.Name : string.Empty,
                    CategoryName = a.Category != null ? a.Category.CateName : string.Empty,
                    SubName = a.Subcategory != null ? a.Subcategory.SubName : string.Empty,
                    Status = a.Status,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                })
                .OrderByDescending(a => a.StartTime)
                .ToList();

            return View(activities);
        }
    }
}
