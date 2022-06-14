using System.ComponentModel.DataAnnotations;

namespace AuthSample.Models
{
    public class LoginModel
    {
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string Password { get; set; }
    }

    public class MakeUserAdminModel
    {
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string UserName { get; set; }
    }
    
    public class ResetUserPasswordModel
    {
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string UserName { get; set; }
    }
}