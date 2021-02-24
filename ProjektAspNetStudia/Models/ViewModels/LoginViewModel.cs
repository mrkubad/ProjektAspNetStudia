using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjektAspNetStudia.Models.ViewModels
{
    public class LoginViewModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
        public string CustomErrorField { get; } = string.Empty; // it is only for placing custom error messages
    }
}
