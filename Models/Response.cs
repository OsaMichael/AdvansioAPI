using System;

namespace AdvanAPI.Models
{
    public class Response
    {
        public string RequestId => $"{Guid.NewGuid().ToString()}";
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public object Data { get; set; }
        public string TransactionId { get; set; }
        public bool Succeeded { get; set; }
     

    }
    public class TransferResponse
    {

        public string AccountNumber { get; set; }
        public decimal CurrentAccountBalance { get; set; }
        public string TransactionId { get; set; }
        public string AccountName { get; set; }
        public string ResponseMessage { get; set; }



    }

    public class AccountBalanceResponse
    {

        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public decimal CurrentAccountBalance { get; set; }




    }
}
