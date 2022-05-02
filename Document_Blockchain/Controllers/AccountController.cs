using Document_Blockchain.Constants;
using Document_Blockchain.IInternalServices;
using Document_Blockchain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Document_Blockchain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [HttpPut("[action]")]
        public async Task<ServiceResponse<LoginOtpResponse>> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return new ServiceResponse<LoginOtpResponse>(400, ApiResponseConstants.invalidInput, null);

            var result = await _unitOfWork.AccountService.Login(model);
            return result;
        }

        [AllowAnonymous]
        [HttpPut("[action]")]
        public async Task<ServiceResponse<LoginResponse>> VerifyOTP(OTPModel model)
        {
            if (!ModelState.IsValid)
                return new ServiceResponse<LoginResponse>(400, ApiResponseConstants.invalidInput, null);

            var result = await _unitOfWork.AccountService.VerifyOTP(model);
            return result;
        }

        [AllowAnonymous]
        [HttpPut("[action]")]
        public async Task<ServiceResponse<LoginOtpResponse>> ResendVerifyOTP(ResendOTPModel model)
        {
            if (!ModelState.IsValid)
                return new ServiceResponse<LoginOtpResponse>(400, ApiResponseConstants.invalidInput, null);

            var result = await _unitOfWork.AccountService.ResendVerifyOTP(model);
            return result;
        }


        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ApiResponse> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var result = await _unitOfWork.AccountService.Register(model);
            return result;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ApiResponse> ResetPassword(ResetPasswordModel model)
        {
            var res = await _unitOfWork.AccountService.ResetPassword(model.Email, model.Token, model.Password);
            return res;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ApiResponse> VerifyEmail(string token)
        {
            var res = await _unitOfWork.AccountService.VerifyEmail(token);
            return res;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ApiResponse> ResendVerifyEmail(string email)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, StatusCodes.Status400BadRequest);

            var res = await _unitOfWork.AccountService.ResendVerifyEmail(email);
            return res;
        }



        [AllowAnonymous]
        [HttpPut("[action]")]
        public async Task<ServiceResponse<LoginResponse>> RefreshToken(string refreshToken)
        {
            if (refreshToken is null)
                return new ServiceResponse<LoginResponse>(400, ApiResponseConstants.refrestTokenMandatory, null);

            var result = await _unitOfWork.AccountService.RefreshToken(refreshToken);
            return result;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ApiResponse> ForgotPassword(string email)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var result = await _unitOfWork.AccountService.ForgotPassword(email);
            return result;
        }


        [Authorize]
        [HttpPatch("[action]")]
        public async Task<ApiResponse> UpdatePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var result = await _unitOfWork.AccountService.ChangePassword(_unitOfWork.UserServices.Email, model);
            return result;
        }
    }
}

