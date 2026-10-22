using System;

namespace Final.Models
{
    public class ActivityViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SubName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
