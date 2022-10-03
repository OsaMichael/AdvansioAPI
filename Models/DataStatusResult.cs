using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.Models
{
    public class DataStatusResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public dynamic Result { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
       
    }
}
