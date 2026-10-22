using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final.Models
{
    public class Subcategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubID { get; set; }        // 主鍵 (PK)

        [Required]
        [StringLength(100)]
        public string SubName { get; set; } = string.Empty;    // 細分類名稱

        public int CateID { get; set; }       // 外鍵 (FK)

        // 關聯屬性 (Navigation property)
        [ForeignKey("CateID")]
        public virtual Category? Category { get; set; }
    }
}
