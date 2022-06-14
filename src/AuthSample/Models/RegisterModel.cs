using System.ComponentModel.DataAnnotations;

namespace AuthSample.Models
{
    public class RegisterModel
    {
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string Password { get; set; }
        
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string DisplayName { get; set; }
    }
}