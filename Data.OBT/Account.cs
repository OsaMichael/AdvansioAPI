using System;

namespace AdvanAPI.Data.OBT
{
    public class Account
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public decimal CurrentAccountBalance { get; set; }
        public AccountType AccountType { get; set; } // This will be an Enum to show if the account to be created iis "Savings" or Current
        public string AccountNumber { get; set; } // we shall generate accountNumber here 

        public string PinHash { get; set; }
        public string PinSalt { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public Role Role { get; set; }

        // now let generate  AccountNumberGenerated let do that in thseame constructor 
        // first let create random obj

        Random rand = new Random();

        public Account()
        {
            AccountNumber = Convert.ToString((long)Math.Floor(rand.NextDouble() * 9_000_000_000 + 1_000_000_000L)); // we did 9_000_000_000 so we could get

            //also AccountName property = FirstName + LastName
            AccountName = $"{FirstName} {LastName}";
        }
    }

    public enum AccountType
    {
        Savings, // savings => 0., current => 1 etc.
        Current,
        Corporate,
        Government
    }
    public enum Role
    {
        UserRole,
        AdminRole

    }
}

