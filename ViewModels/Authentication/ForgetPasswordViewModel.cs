using System.ComponentModel.DataAnnotations;

namespace SLBFEMS.ViewModels.Authentication
{
    public class ForgetPasswordViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}
