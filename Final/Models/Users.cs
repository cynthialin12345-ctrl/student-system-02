using System.ComponentModel.DataAnnotations;

namespace Final.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "請輸入使用者名稱.")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入Email.")]
        [EmailAddress(ErrorMessage = "請輸入有效的Email.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入密碼.")]
        [MinLength(6, ErrorMessage = "密碼至少需要6個字.")]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}
