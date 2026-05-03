using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Models.ViewModels
{
    public class LoginViewModel {
        [Required(ErrorMessage = "Email or Name is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
