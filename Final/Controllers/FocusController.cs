using Final.Data;
using Final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Final.Controllers
{
    public class FocusController : Controller
    {
        private readonly CmsContext _context;
        private const string UserIdSessionKey = "UserID";

        public FocusController(CmsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? currentUserId = HttpContext.Session.GetInt32(UserIdSessionKey);
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index), "Focus") });
            }

            var activeActivity = _context.Activity
                .AsNoTracking()
                .Where(a => a.UserID == currentUserId.Value && a.Status == "進行中")
                .OrderByDescending(a => a.StartTime)
                .FirstOrDefault();

            ViewBag.CategoryList = _context.Category.ToList();
            ViewBag.TodayDate = DateTime.Now.ToString("yyyy 年 MM 月 dd 日");
            ViewBag.TodayDayOfWeek = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-TW"));
            ViewBag.IsFocusing = activeActivity != null;
            ViewBag.FocusStartTime = activeActivity?.StartTime.ToString("O");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Start(int cateID, string subName)
        {
            int? currentUserId = HttpContext.Session.GetInt32(UserIdSessionKey);
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index), "Focus") });
            }

            bool hasActiveActivity = _context.Activity
                .Any(a => a.UserID == currentUserId.Value && a.Status == "進行中");
            if (hasActiveActivity)
            {
                return RedirectToAction(nameof(Index));
            }

            bool categoryExists = _context.Category.Any(c => c.CateID == cateID);
            if (!categoryExists)
            {
                return RedirectToAction(nameof(Index));
            }

            int? finalSubID = null;
            subName = subName?.Trim() ?? string.Empty;

            if (!string.IsNullOrEmpty(subName))
            {
                var existingSub = _context.Subcategory
                    .FirstOrDefault(s => s.SubName == subName && s.CateID == cateID);

                if (existingSub != null)
                {
                    finalSubID = existingSub.SubID;
                }
                else
                {
                    Subcategory newSub = new Subcategory
                    {
                        SubName = subName,
                        CateID = cateID
                    };
                    _context.Subcategory.Add(newSub);
                    _context.SaveChanges();

                    finalSubID = newSub.SubID;
                }
            }

            // 建立活動紀錄
            Activity activity = new Activity
            {
                UserID = currentUserId.Value,
                CateID = cateID,
                SubID = finalSubID,
                StartTime = DateTime.Now,
                EndTime = null,
                Status = "進行中"
            };

            _context.Activity.Add(activity);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Stop()
        {
            int? currentUserId = HttpContext.Session.GetInt32(UserIdSessionKey);
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index), "Focus") });
            }

            var now = DateTime.Now;
            var activities = _context.Activity
                .Where(a => a.UserID == currentUserId.Value)
                .Where(a => a.Status == "進行中")
                .OrderByDescending(a => a.StartTime)
                .ToList();

            foreach (var activity in activities)
            {
                activity.EndTime = now;
                activity.Status = "已完成";
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
