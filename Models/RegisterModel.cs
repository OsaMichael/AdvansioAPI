using AdvanAPI.Data.OBT;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.Models
{
    public class RegisterModel
    {


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        //public AccountType AccountType { get; set; } // This will be an Enum to show if the account to be created is "Savings" or Current
                                                     // public string AccountNumberGenerated { get; set; } // we shall generate accountNumber here 
                                                     // w'll also store the hash and salt of the account
        //public DateTime DateCreated { get; set; }
        //public DateTime DateLastUpdated { get; set; }
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin must not be more than 4 digits")] // it should be a 4-digit string
        public string Pin { get; set; }
        [Compare("Pin", ErrorMessage = "Pin do not match")]
        public string ConfirmPin { get; set; }// we want to compare both of them

    }
}
