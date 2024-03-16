using System.ComponentModel.DataAnnotations;

namespace gitaAPI.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        //[Required(ErrorMessage = "email  is required")]
        //public string? email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        
    }
}
