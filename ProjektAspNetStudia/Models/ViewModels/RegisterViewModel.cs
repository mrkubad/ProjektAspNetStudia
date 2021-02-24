using System.ComponentModel.DataAnnotations;

namespace ProjektAspNetStudia.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email {get ; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(5)]
        [MaxLength(16)]
        public string Password { get; set; }
    }
}
