using System;
using System.ComponentModel.DataAnnotations;
using SLBFEMS.Enums;

namespace SLBFEMS.ViewModels.Authentication
{
    public class NewUserViewModel
    {
        [Required(ErrorMessage = "NIC is Required")]
        public string NIC { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Genders Gender { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; }

        [Required(ErrorMessage ="Role is required")]
        public UserRoles Role { get; set; }

    }
}
