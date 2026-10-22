using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final.Models;
using Final.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Final.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly CmsContext _context;
        private const string UserIdSessionKey = "UserID";

        public StatisticsController(CmsContext context)
        {
            _context = context;
        }

        // 用於讀取視覺化頁面
        public IActionResult Index()
        {
            if (!HttpContext.Session.GetInt32(UserIdSessionKey).HasValue)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index), "Statistics") });
            }

            return View("~/Views/Home/Analytics.cshtml");
        }

        public IActionResult Daily()
        {
            int? currentUserId = HttpContext.Session.GetInt32(UserIdSessionKey);
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var result = _context.Activity
                .AsNoTracking()
                .Where(a => a.UserID == currentUserId.Value && a.EndTime != null)
                .ToList() // 轉至記憶體以處理 DateTime 運算
                .GroupBy(a => a.StartTime.Date)
                .Select(g => new {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    TotalMinutes = Math.Round(g.Sum(a => (a.EndTime!.Value - a.StartTime).TotalMinutes), 1)
                })
                .OrderBy(a => a.Date)
                .ToList();

            return Json(new { data = result });
        }

        // 提供給前端 Dashboard 的實時統計 API 接口
        public IActionResult GetDashboardData()
        {
            int? currentUserId = HttpContext.Session.GetInt32(UserIdSessionKey);
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var today = DateTime.Today;

            // 1. 本日累積總時間 (今日已完成的活動)
            var todayActivities = _context.Activity
                .AsNoTracking()
                .Include(a => a.Category)
                .Where(a => a.UserID == currentUserId.Value && a.EndTime != null && a.StartTime >= today && a.StartTime < today.AddDays(1))
                .ToList();

            double totalMinutesToday = todayActivities.Sum(a => (a.EndTime!.Value - a.StartTime).TotalMinutes);
            int totalHours = (int)(totalMinutesToday / 60);
            int totalMinutes = (int)(totalMinutesToday % 60);

            // 2. 分類統計 (對應：課業、工作、娛樂、休息)
            string[] categoryNames = { "課業", "工作", "娛樂", "休息" };
            var categoryHours = new double[4];
            for (int i = 0; i < categoryNames.Length; i++)
            {
                var catAct = todayActivities.Where(a => a.Category?.CateName == categoryNames[i]);
                categoryHours[i] = Math.Round(catAct.Sum(a => (a.EndTime!.Value - a.StartTime).TotalMinutes) / 60.0, 1);
            }

            // 3. 週統計 (對應：週一 到 週日)
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = today.AddDays(-1 * diff);
            var endOfWeek = startOfWeek.AddDays(7);

            var weeklyActivities = _context.Activity
                .AsNoTracking()
                .Where(a => a.UserID == currentUserId.Value && a.EndTime != null && a.StartTime >= startOfWeek && a.StartTime < endOfWeek)
                .ToList();

            var weeklyHours = new double[7];
            for (int i = 0; i < 7; i++)
            {
                var day = startOfWeek.AddDays(i);
                var dayAct = weeklyActivities.Where(a => a.StartTime.Date == day.Date);
                weeklyHours[i] = Math.Round(dayAct.Sum(a => (a.EndTime!.Value - a.StartTime).TotalMinutes) / 60.0, 1);
            }

            // 4. 細分類統計 (前 5 名最常專注的任務)
            var subcategoryQuery = _context.Activity
                .AsNoTracking()
                .Where(a => a.UserID == currentUserId.Value && a.EndTime != null && a.SubID != null)
                .Include(a => a.Subcategory)
                .ToList() // 轉至記憶體以加載關聯實體
                .GroupBy(a => a.Subcategory != null ? a.Subcategory.SubName : "無類別")
                .Select(g => new {
                    SubName = g.Key,
                    Hours = Math.Round(g.Sum(a => (a.EndTime!.Value - a.StartTime).TotalMinutes) / 60.0, 1)
                })
                .OrderByDescending(x => x.Hours)
                .Take(5)
                .ToList();

            var subCategoryLabels = subcategoryQuery.Select(x => x.SubName).ToArray();
            var subCategoryHours = subcategoryQuery.Select(x => x.Hours).ToArray();

            // 若完全無細分類資料，提供空狀態避免圖表全白
            if (subCategoryLabels.Length == 0)
            {
                subCategoryLabels = new string[] { "尚未有細分類紀錄" };
                subCategoryHours = new double[] { 0 };
            }

            var dashboardData = new
            {
                TotalHours = totalHours,
                TotalMinutes = totalMinutes,
                CategoryHours = categoryHours, // 4 個元素對應 課業, 工作, 娛樂, 休息
                WeeklyHours = weeklyHours,     // 7 個元素對應 週一至週日
                SubCategoryLabels = subCategoryLabels,
                SubCategoryHours = subCategoryHours
            };

            return Json(dashboardData);
        }
    }
}
