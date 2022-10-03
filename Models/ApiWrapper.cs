using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.ApiWrapper
{
    public class Response
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSuccessful"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        public Response(bool isSuccessful, object data, string message)
        {
            IsSuccessful = isSuccessful;
            Data = data;
            Message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccessful { get; }


        /// <summary>
        /// 
        /// </summary>
        public string Message { get; }




    }
    /// <summary>
    /// 
    /// </summary>
    public static class ResponseExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="succeed"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Response ToResponse(this object data, bool succeed = true, string message = "Operation Successful")
        {
            return new Response(succeed, data, message);
        }
    }
}
