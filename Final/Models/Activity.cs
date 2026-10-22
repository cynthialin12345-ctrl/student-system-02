using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final.Models
{
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityID { get; set; }   // 主鍵 (PK)

        public int UserID { get; set; }       // 外鍵 (FK)

        public int CateID { get; set; }       // 外鍵 (FK)

        public int? SubID { get; set; }       // 外鍵 (FK)，允許為 NULL

        public DateTime StartTime { get; set; } // 開始時間
        public DateTime? EndTime { get; set; }   // 結束時間
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;    // 任務狀態 (進行中 / 已完成)

        // 關聯屬性 (Navigation properties)
        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }

        [ForeignKey("CateID")]
        public virtual Category? Category { get; set; }

        [ForeignKey("SubID")]
        public virtual Subcategory? Subcategory { get; set; }
    }
}
