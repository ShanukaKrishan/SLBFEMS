using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SLBFEMS.Enums;

namespace SLBFEMS.ViewModels.Authentication
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "User Info is required")]
        public UserInfoViewModel UserInfo { get; set; }

        public JobSeekerDataViewModel JobSeekerData { get; set; } = null;

    }

    public class UserInfoViewModel
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
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage ="Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public virtual string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public UserRoles Role { get; set; }

    }

    public class JobSeekerDataViewModel
    {
        [Required(ErrorMessage = "Latitude is required")]
        public string CurrentLat { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        public string CurrentLong { get; set; }

        [Required(ErrorMessage = "Profession is required")]
        public string Profession { get; set; }

        [Required(ErrorMessage = "Affiliations are required")]
        public List<AffiliationDataViewModel> Affiliations { get; set; }

        [Required(ErrorMessage = "Qualificatins are required")]
        public List<string> Qualifications { get; set; }

        [Required(ErrorMessage = "Education data is required")]
        public List<EducationDataViewModel> Education { get; set; } 

        [Required(ErrorMessage = "CV file name is required")]
        public string CvFileName { get; set; }
    }

    public class AffiliationDataViewModel
    {
        [Required(ErrorMessage = "Start date is required")]
        public string Start { get; set; }

        public string End { get; set; } = "Present";

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Organization is required")]
        public string Organization { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
    }

    public class EducationDataViewModel
    {
        [Required(ErrorMessage = "Start date is required")]
        public string Start { get; set; }

        public string End { get; set; } = "Present";

        [Required(ErrorMessage = "Institution name is required")]
        public string Name { get; set; }
    }
}
