namespace AdvanAPI.Models
{
    public class AccountDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public decimal CurrentAccountBalance { get; set; }
        public string AccountNumber { get; set; }
        public string Roles { get; set; }
        public bool Succeeded { get; set; }
    }
    public class AccountDTOResp
    {
 
        //public decimal CurrentAccountBalance { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
  
        public string PinHash { get; set; }

        public string PinSalt { get; set; }
        public bool Succeeded { get; set; }
    }
}
