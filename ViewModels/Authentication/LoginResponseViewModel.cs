using System;
using System.Collections.Generic;

namespace SLBFEMS.ViewModels.Authentication
{
    public class LoginResponseViewModel
    {
        public string Token { get; set; }

        public string Id { get; set; }

        public string NIC { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public List<string> Role { get; set; }

        public string Email { get; set; }

        public DateTime expiration { get; set;}
    }
}
