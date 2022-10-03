using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class TransferModel
    {
        public string FromAccount { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
