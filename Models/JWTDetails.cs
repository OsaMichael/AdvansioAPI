using System;

namespace AdvanAPI.Models
{
    public class JWTDetails
    {
        public string token { get; set; }
        public DateTime expiration { get; set; }
        public string Message { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public bool status { get; set; }
    }

    public class LoginError
    {
        public string Message { get; set; }

    }
}
