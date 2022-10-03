using AdvanAPI.Data.OBT;
using AdvanAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvanAPI.Repository
{
    public interface IAccountRepo
    {
        //Task<DataStatusResult> Resgister(RegisterModel register);
        //Task<DataStatusResult> Resgister(Account account, string Pin, string ConfirmPin);
        //Task<DataStatusResult> SignIn(LoginModel login);
        //Task<DataStatusResult> Resgister(Account account, string Pin, string ConfirmPin);
        Task<Response> MakeFundTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin);
        Task<Response> GetBalanceByAccountNumber(string AccountNumber);
        Task<Response> UserLogin(string userName, string password);
        Task<Response> Resgister(Account account, string Pin, string ConfirmPin);
        Task<List<AccountResp>> GetDashBoard();
       // Task<Response> Resgister(Account account, string Pin, string ConfirmPin);
    }
}
