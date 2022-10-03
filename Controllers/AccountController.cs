using AdvanAPI.Data.OBT;
using AdvanAPI.Models;
using AdvanAPI.Repository;
using AdvanAPI.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdvanAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly ILogger<AccountController> _logger;
        private readonly IAccountRepo _accountService;
          private readonly IMapper _mapper;
        public AccountController(IAccountRepo iaccount, ILogger<AccountController> logger, IMapper mapper)
        {
            _accountService = iaccount;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel register)
        {
            try
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(register.Email);
                if (!match.Success)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new APIiResponse
                    {
                        code = GeneralStatusCodes.Status_OperationFailed.code,
                        message = "incorrect email format"
                    });
                }

                var account = _mapper.Map<Account>(register);
                var result = await _accountService.Resgister(account, register.Pin, register.ConfirmPin);

                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status200OK, result.Data);
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new APIiResponse
                    {
                        code = GeneralStatusCodes.Status_ShitHappens_General.code,
                        message = GeneralStatusCodes.Status_ShitHappens_General.message

                    });
                }

            }

            catch (Exception ex)
            {
                _logger.LogError($"An error occurred.", ex);
                return StatusCode(StatusCodes.Status400BadRequest, new APIiResponse { code = GeneralStatusCodes.Status_ShitHappens_General.code, message = GeneralStatusCodes.Status_ShitHappens_General.message });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(login.UserName) && String.IsNullOrWhiteSpace(login.Password))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new APIiResponse
                    {
                        code = GeneralStatusCodes.Status_OperationFailed.code,
                        message = "all field are required"
                    });
                }

                var result = await _accountService.UserLogin(login.UserName, login.Password);

                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status200OK, result);
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new APIiResponse
                    {
                        code = GeneralStatusCodes.Status_ShitHappens_General.code,
                        message = GeneralStatusCodes.Status_ShitHappens_General.message

                    });
                }

            }

            catch (Exception ex)
            {
                _logger.LogError($"An error occurred.", ex);
                return StatusCode(StatusCodes.Status400BadRequest, new APIiResponse { code = GeneralStatusCodes.Status_ShitHappens_General.code, message = GeneralStatusCodes.Status_ShitHappens_General.message });
            }
        }
        [HttpPost]
        [Route("fund_transfer")]
        public async Task<IActionResult> WalletTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            try
            {
                if (!Regex.IsMatch(FromAccount, @"^[0][1-9]\d[9]$|^[1-9]\d{9}$") || !Regex.IsMatch(ToAccount, @"^[0][1-9]\d[9]$|^[1-9]\d{9}$")) return BadRequest("Account number must be 10-digit");

                var result = await _accountService.MakeFundTransfer(FromAccount, ToAccount, Amount, TransactionPin);

                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status200OK, result);
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new APIiResponse
                    {
                        code = GeneralStatusCodes.Status_ShitHappens_General.code,
                        message = GeneralStatusCodes.Status_ShitHappens_General.message

                    });
                }

            }

            catch (Exception ex)
            {
                _logger.LogError($"An error occurred.", ex);
                return StatusCode(StatusCodes.Status400BadRequest, new APIiResponse { code = GeneralStatusCodes.Status_ShitHappens_General.code, message = GeneralStatusCodes.Status_ShitHappens_General.message });
            }
        }

        [HttpGet]
        [Route("wallet_balance")]
        public async Task<IActionResult> GetWelletBalance([FromQuery] string accountNumber)
        {
            try
            {
                if (!Regex.IsMatch(accountNumber, @"^[0][1-9]\d[9]$|^[1-9]\d{9}$")) return BadRequest("Account number must be 10-digit");

                var result = await _accountService.GetBalanceByAccountNumber(accountNumber);

                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status200OK, result.Data);
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new APIiResponse
                    {
                        code = GeneralStatusCodes.Status_ShitHappens_General.code,
                        message = GeneralStatusCodes.Status_ShitHappens_General.message

                    });
                }

            }

            catch (Exception ex)
            {
                _logger.LogError($"An error occurred.", ex);
                return StatusCode(StatusCodes.Status400BadRequest, new APIiResponse { code = GeneralStatusCodes.Status_ShitHappens_General.code, message = GeneralStatusCodes.Status_ShitHappens_General.message });
            }
        }
        
    }
}