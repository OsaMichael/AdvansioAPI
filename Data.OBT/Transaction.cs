using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvanAPI.Data.OBT
{
    public class Transaction
    {
        public int Id { get; set; }
        public string TransactionUniqueReference { get; set; } // thsi we will generate in every insatnace off this class
        public decimal TransactionAmount { get; set; }
        public TranStatus TransactionStatus { get; set; }// this is an enum to lets create it
        public bool IsSuccessful => TransactionStatus.Equals(TranStatus.Success);// this guy depends on the value of TransactionStatus
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public string TransactionParticulars { get; set; }
        public TranType TransactionType { get; set; } // this is another enume
        public DateTime TransactionDate { get; set; }


        public Transaction()
        {
            TransactionUniqueReference = $"{Guid.NewGuid().ToString().Replace(" ", " ").Substring(1, 27)}"; /// we will use guid to generate it
        }
    }
    public enum TranStatus
    {
        Failed,
        Success,
        Error
    }

    public enum TranType
    {
        Deposit,
        Withdrwawl,
        Transfer
    }
}
