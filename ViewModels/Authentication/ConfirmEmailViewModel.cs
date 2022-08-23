using System.ComponentModel.DataAnnotations;

namespace SLBFEMS.ViewModels.Authentication
{
    public class ConfirmEmailViewModel
    {
        [Required(ErrorMessage = "NIC is Required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Token is Required")]
        public string Token { get; set; }
    }
}
