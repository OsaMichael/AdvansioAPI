using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.Models
{
    public class DbReturnDto<T>
    {
        public virtual long id { get; set; }
        public virtual bool isSuccess { get; set; }
        public virtual string message { get; set; }
        public T Data { get; set; }
    }
}
