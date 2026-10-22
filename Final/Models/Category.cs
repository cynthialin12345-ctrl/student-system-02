using System.ComponentModel.DataAnnotations;

namespace Final.Models
{
    public class Category
    {
        [Key]
        public int CateID { get; set; }       // 主鍵 (PK)

        [Required]
        [StringLength(50)]
        public string CateName { get; set; } = string.Empty;   // 課業、工作、娛樂、休息
    }
}
