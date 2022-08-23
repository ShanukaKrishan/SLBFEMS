using Microsoft.AspNetCore.Identity;
using SLBFEMS.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SLBFEMS.Models
{
    public class ApplicationUserModel : IdentityUser
    {
        [Key]
        public string NIC { get; set; }

        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        [PersonalData]
        public Genders Gender { get; set; }

        [PersonalData]
        public DateTime Birthday { get; set; }

        [PersonalData]
        public string Address { get; set; }

        public bool DeleteStatus { get; set; } = false;

    }
}
