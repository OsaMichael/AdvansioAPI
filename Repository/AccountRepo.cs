using AdvanAPI.Data.OBT;
using AdvanAPI.Models;
using AdvanAPI.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdvanAPI.Repository
{
    public class AccountRepo: IAccountRepo
    {
        readonly ILogger<AccountRepo> _logger;
        private readonly IConfiguration _config;
        public AccountRepo(ILogger<AccountRepo> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<Response> Resgister(Account account, string Pin, string ConfirmPin)
        {
            var res = new Response();
            //Response response = new Response();
            try 
            {
                if (!Pin.Equals(ConfirmPin)) throw new ApplicationException("Pin do not match Pin");

                var pinHashing = Hashing2(Pin);
                if (pinHashing == null) throw new Exception(" an error has occur");
                // hash password 
                var passwordHashing = Hashing(account.Password);
                if (passwordHashing == null) throw new Exception(" an error has occur");
                // fund the account
                decimal currentBal = Convert.ToDecimal(5000);
                account.CurrentAccountBalance = currentBal;

                string connString = _config.GetValue<string>("ConnectionStrings:dbConnection");

                var OraConn = DatabaseConnection.SqlDatabaseCreateConnection(connString.ToString(), true);

                var paras = new Dictionary<string, DatabaseParameterWrappers>();

                account.AccountName = account.FirstName + " " + account.LastName;

                paras.Add("@Email", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(account.Email) ? "" : account.Email.Trim()));
                paras.Add("@FirstName", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(account.FirstName) ? "" : account.FirstName.Trim()));
                paras.Add("@Password", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(passwordHashing) ? "" : passwordHashing.Trim()));
                paras.Add("@LastName", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(account.LastName) ? "" : account.LastName.Trim()));
                paras.Add("@PhoneNumber", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(account.PhoneNumber) ? "" : account.PhoneNumber.Trim()));
                paras.Add("@hashPin", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(pinHashing) ? "" : pinHashing.Trim()));
               // paras.Add("@hashSalt", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(hashSalt) ? "" : hashSalt.Trim()));
                paras.Add("@AccountName", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(account.AccountName) ? "" : account.AccountName.Trim()));
                paras.Add("@AccountNumber", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(account.AccountNumber) ? "" : account.AccountNumber.Trim()));
                paras.Add("@AccountType", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(account.AccountType.ToString()) ? "" : account.AccountType.ToString().Trim()));
                paras.Add("@CurrentAccountBalance", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(account.CurrentAccountBalance.ToString()) ? "" : account.CurrentAccountBalance.ToString().Trim()));
                paras.Add("@Role", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(account.Role.ToString()) ? "" : account.Role.ToString().Trim()));

                _logger.LogInformation($"{account}: About going to Database with params {JsonConvert.SerializeObject(paras)}");
                var reg = await DapperUtilities<AccountDTO>.GetSingleObjectAsync(OraConn, paras, "proc_RegisterNewAccount", CommandType.StoredProcedure);
                _logger.LogInformation($"{account}: Came back from Database with {JsonConvert.SerializeObject(reg)}");
                if (reg.AccountName != null)
                {
                    res.ResponseCode = "00";
                    res.ResponseMessage = "Registered successful!";
                    res.Data = new TransferResponse { CurrentAccountBalance = reg.CurrentAccountBalance, AccountNumber = account.AccountNumber, AccountName = reg.AccountName, ResponseMessage  = res.ResponseMessage };
                    res.Succeeded = true;
                }
                    else
                {

                    res.Data = null;
                    res.Succeeded = false;
                    res.ResponseMessage = GeneralStatusCodes.Status_Exception.message + "Couldn't register user OR User already exist";
                    res.ResponseCode = GeneralStatusCodes.Status_Exception.code;


                }
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError($"An exception occured:", e);
                return res;
            }


        }
       
        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        private static  string Hashing(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // combine both 
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // befroe saving to db convert to string
            string savedHash = Convert.ToBase64String(hashBytes);
            return savedHash;
        }
        private static string Hashing2(string pin)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(pin, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // combine both 
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // befroe saving to db convert to string
            string savedHash = Convert.ToBase64String(hashBytes);
            return savedHash;
        }

        public async Task<Response> UserLogin(string userName, string password)
        {
            Response response = new Response();
            /* Fetch the stored value */
            //string savedPasswordHash = DBContext.GetUser(u => u.UserName == user).Password;
            try
            {
                string connString = _config.GetValue<string>("ConnectionStrings:dbConnection");

            var OraConn = DatabaseConnection.SqlDatabaseCreateConnection(connString.ToString(), true);

            var paras = new Dictionary<string, DatabaseParameterWrappers>();

            paras.Add("@Email", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(userName) ? "" : userName.Trim()));
            paras.Add("@Password", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(password) ? "" : password.Trim()));
            _logger.LogInformation($"{userName}: About going to Database to verify password {userName}");
            var user = await DapperUtilities<AccountDTO>.GetSingleObjectAsync(OraConn, paras, "pro_VerifyPassword", CommandType.StoredProcedure);
            _logger.LogInformation($"{userName}: Came back from Database with {JsonConvert.SerializeObject(user)}");

            if (user.Password == null)
                return null;

      
            byte[] hashBytes = Convert.FromBase64String(user.Password);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                {
                    response.Succeeded = false;
                    //response.Data = new UnauthorizedAccessException();
                    throw new UnauthorizedAccessException();
                }
                else
                {
                        // gerate toke

                        var token = Token(user.AccountName, user.Email, user.Roles);

                    response.Data = new JWTDetails {token = token.token, expiration = token.expiration, FullName = token.FullName, Role = token.Role, Message = token.Message, status = token.status};
                    response.Succeeded = true;
                    response.ResponseMessage = "Login successful";
                    response.ResponseCode = "00";
                    return response;
                }
            }
            catch (Exception xx)
            {
                _logger.LogError($"An exception occured:", xx);
                throw xx;
            }
            return response;
        }


        public async  Task<Response> MakeFundTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            string connString = _config.GetValue<string>("ConnectionStrings:dbConnection");
            var OraConn = DatabaseConnection.SqlDatabaseCreateConnection(connString.ToString(), true);
            // first check that user - account owner is valid,
            // we'll need authenticate
            var authUser = await Authenticate(FromAccount, TransactionPin);

            if (authUser.Succeeded == false) throw new ApplicationException("Invalid PIN credentials");

            // so validation passes
            try
            {
                // for deposit, our bankSetlementAccount is the destination getting money from the users account
                sourceAccount = await GetByAccountNumber(FromAccount);
                if(sourceAccount.AccountNumber == null) throw new ApplicationException("Invalid sourceAccount AccountNumber credentials");
                destinationAccount = await GetByAccountNumber(ToAccount);
                if(destinationAccount.AccountNumber == null) throw new ApplicationException("Invalid destinationAccount AccountNumber credentials");
                // now let update their account balance
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                //check if there is update     

                var paras = new Dictionary<string, DatabaseParameterWrappers>();

                paras.Add("@DestinationAccountNumber", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(destinationAccount.AccountNumber) ? "" : destinationAccount.AccountNumber.Trim()));
                paras.Add("@destinationAmount", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(destinationAccount.CurrentAccountBalance.ToString()) ? "" : destinationAccount.CurrentAccountBalance.ToString().Trim()));
                paras.Add("@SourceAccountNumber", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(sourceAccount.AccountNumber) ? "" : sourceAccount.AccountNumber.Trim()));
                paras.Add("@sourecAmount", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(sourceAccount.CurrentAccountBalance.ToString()) ? "" : sourceAccount.CurrentAccountBalance.ToString().Trim()));

                _logger.LogInformation($" from this AccountNumber {sourceAccount}: About going to Database to credit AccountNumber {destinationAccount}");
                var account = await DapperUtilities<Account>.GetSingleObjectAsync(OraConn, paras, "pro_CreditDestinationAccount", CommandType.StoredProcedure);
                _logger.LogInformation($"Came back from Database with {JsonConvert.SerializeObject(account)}");

                if(account != null)
                {
   
                    transaction.TransactionStatus = TranStatus.Success;
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Transaction successful!";
                    response.Data = new TransferResponse {AccountName = account.AccountName,  CurrentAccountBalance  = account.CurrentAccountBalance, AccountNumber = account.AccountNumber , TransactionId = transaction.TransactionUniqueReference};
                    response.Succeeded = true;
                    response.TransactionId = transaction.TransactionUniqueReference;
                }
                else
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Transaction failed!";
                    response.Data = null;
                    response.Succeeded = false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERRROR OCCURED... => {ex.Message}");
                throw ex;
            }
            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = FromAccount;
            transaction.TransactionDestinationAccount = ToAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
             
              //transaction.TransactionUniqueReference;
                transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT =>  {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => { transaction.TransactionDate}  FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus} ";

                string connStrings = _config.GetValue<string>("ConnectionStrings:dbConnection");
                var OraCon = DatabaseConnection.SqlDatabaseCreateConnection(connStrings.ToString(), true);

                var para = new Dictionary<string, DatabaseParameterWrappers>();
             
            para.Add("@TransactionType", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(transaction.TransactionType.ToString()) ? "" : transaction.TransactionType.ToString().Trim()));
            para.Add("@TransactionSourceAccount", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(transaction.TransactionSourceAccount) ? "" : transaction.TransactionSourceAccount.Trim()));
            para.Add("@TransactionDestinationAccount", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(transaction.TransactionDestinationAccount) ? "" : transaction.TransactionDestinationAccount.Trim()));
            para.Add("@TransactionAmount", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(transaction.TransactionAmount.ToString()) ? "" : transaction.TransactionAmount.ToString().Trim()));
            para.Add("@TransactionUniqueReference", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(transaction.TransactionUniqueReference) ? "" : transaction.TransactionUniqueReference.Trim()));
            para.Add("@TransactionStatus", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(transaction.TransactionStatus.ToString()) ? "" : transaction.TransactionStatus.ToString().Trim()));
            para.Add("@IsSuccessful", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(transaction.IsSuccessful.ToString()) ? "" : transaction.IsSuccessful.ToString().Trim()));

                _logger.LogInformation($" from this AccountNumber {transaction}: About going to Database to credit AccountNumber {transaction}");
            var account1 = await DapperUtilities<Account>.GetSingleObjectAsync(OraCon, para, "pro_TransactionHistory", CommandType.StoredProcedure);
            _logger.LogInformation($"Came back from Database with {JsonConvert.SerializeObject(transaction)}");

            return response;

        
        }


        public async Task<AccountDTOResp> Authenticate(string AccountNumber, string Pin)
        {
            // let make authentication
            // does account exist for that number
            var response = new AccountDTOResp();
            try
            {

                string connString = _config.GetValue<string>("ConnectionStrings:dbConnection");

                var OraConn = DatabaseConnection.SqlDatabaseCreateConnection(connString.ToString(), true);

                var paras = new Dictionary<string, DatabaseParameterWrappers>();

                paras.Add("@AccountNumber", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(AccountNumber) ? "" : AccountNumber.Trim()));
                //paras.Add("@Pin", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(Pin) ? "" : Pin.Trim()));
                _logger.LogInformation($"{AccountNumber}: About going to Database to verify accountNumber {AccountNumber}");
                var account = await DapperUtilities<AccountDTOResp>.GetSingleObjectAsync(OraConn, paras, "pro_VerifyAccountNumber", CommandType.StoredProcedure);
                _logger.LogInformation($"{AccountNumber}: Came back from Database with {JsonConvert.SerializeObject(account)}");

                if (account.PinHash == null)
                    return null;


            byte[] hashBytes = Convert.FromBase64String(account.PinHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(Pin, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                {
                    response.Succeeded = false;

                    throw new UnauthorizedAccessException();
                }
                else
                {
                    response.Succeeded = true;
                    response.PinHash = account.PinHash;
                    return response;
                }
        }

            catch (Exception xx)
            {
                _logger.LogError($"An exception occured:", xx);
                throw xx;

            }
            return response;
            // Ok so  Autghentication is passesed
           
        }


        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ApplicationException("Pin");
            // now let verify Pin

            using (var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for (int i = 0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;

                }

            }
            return true;
        }

        public async Task<Account> GetByAccountNumber(string AccountNumber)
        {

            try
            {
                string connString = _config.GetValue<string>("ConnectionStrings:dbConnection");

                var OraConn = DatabaseConnection.SqlDatabaseCreateConnection(connString.ToString(), true);

                var paras = new Dictionary<string, DatabaseParameterWrappers>();

                paras.Add("@AccountNumber", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(AccountNumber) ? "" : AccountNumber.Trim()));
                _logger.LogInformation($"{AccountNumber}: About going to Database to verify accountNumber {AccountNumber}");
                var account = await DapperUtilities<Account>.GetSingleObjectAsync(OraConn, paras, "pro_CheckAccountNumber", CommandType.StoredProcedure);
                _logger.LogInformation($"{AccountNumber}: Came back from Database with {JsonConvert.SerializeObject(account)}");

                if (account.AccountNumber == null)
                    return null;

                return account;
            }

          catch(Exception xx)
            {
                throw xx;
            }
            

            
        }

        public async Task<Response> GetBalanceByAccountNumber(string AccountNumber)
        {
            Response response = new Response();
            try { 
            // var account = _context.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).FirstOrDefault();
            string connString = _config.GetValue<string>("ConnectionStrings:dbConnection");

            var OraConn = DatabaseConnection.SqlDatabaseCreateConnection(connString.ToString(), true);

            var paras = new Dictionary<string, DatabaseParameterWrappers>();

            paras.Add("@AccountNumber", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(AccountNumber) ? "" : AccountNumber.Trim()));
            _logger.LogInformation($"{AccountNumber}: About going to Database to verify accountNumber {AccountNumber}");
            var account = await DapperUtilities<Account>.GetSingleObjectAsync(OraConn, paras, "pro_GetBalanceByAccountNumber", CommandType.StoredProcedure);
            _logger.LogInformation($"{AccountNumber}: Came back from Database with {JsonConvert.SerializeObject(account)}");

            if (account == null)
            {
                response.Succeeded = false;
               // return null;
            }
               
            else
            {
                response.Data = new AccountBalanceResponse { AccountName = account.AccountName, AccountNumber = account.AccountNumber, CurrentAccountBalance = account.CurrentAccountBalance};
                response.Succeeded = true;
                response.ResponseCode = GeneralStatusCodes.Status_Success.code;
                response.ResponseMessage = GeneralStatusCodes.Status_Success.message;
            }
           }

          catch(Exception xx)
            {
                throw xx;
            }

            return response;
        }

        public JWTDetails Token(string name, string email, string role)
        {
            var res = new JWTDetails();
            if (!string.IsNullOrEmpty(name) && (!string.IsNullOrEmpty(email)) && (!string.IsNullOrEmpty(role)))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                //foreach (var userRole in roles)
                //{
                //    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                //}

                authClaims.Add(new Claim(ClaimTypes.Role, role));

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _config["JWT:ValidIssuer"],
                    audience: _config["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );


                var resp =  new JWTDetails
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    Message = "login successful",
                    Role = role,
                    FullName = name,
                    status = true

                };

                 res = resp;
            }
            else
            {
                res.Message = "login failed";
                res.status = false;
            }
            return res;
        }

        public async Task<List<AccountResp>> GetDashBoard()
        {

            try
            {
                string connString = _config.GetValue<string>("ConnectionStrings:dbConnection");

                var OraConn = DatabaseConnection.SqlDatabaseCreateConnection(connString.ToString(), true);

                var paras = new Dictionary<string, DatabaseParameterWrappers>();
                var channel = "advansio";
                paras.Add("@channel", new DatabaseParameterWrappers(string.IsNullOrWhiteSpace(channel) ? "" : channel.Trim()));
                var account =  DapperUtilities<AccountResp>.GetList(OraConn, "pro_allAccount", CommandType.StoredProcedure);
                _logger.LogInformation($"{account}: Came back from Database with {JsonConvert.SerializeObject(account)}");

              //  var acct = JsonConvert.DeserializeObject<List<AccountResp>>(account);
                if (account == null)
                    return null;

                
                return account;
            }

            catch (Exception xx)
            {
                throw xx;
            }



        }

    }
}
