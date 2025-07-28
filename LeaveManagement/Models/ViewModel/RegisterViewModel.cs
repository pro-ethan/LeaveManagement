using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name ="Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Minimum 5 characters you should provide")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match")]
        [Display(Name = "Confirm Password")]

        public string ConfirmPassword { get; set; }

        public string? role { get; set; }
    }
}
