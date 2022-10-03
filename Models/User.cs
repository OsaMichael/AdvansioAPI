using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string BVN { get; set; }
        public string FullAddress { get; set; }
        public string AccountNumber { get; set; }
        public string CardPan { get; set; }
        public string Otp { get; set; }
    }
}
