using Document_Blockchain.Models;
using System.Threading.Tasks;

namespace Document_Blockchain.IInternalServices
{
    public interface IAccountService
    {
        Task<ApiResponse> Register(RegisterModel model);

        Task<ServiceResponse<LoginOtpResponse>> Login(LoginModel model);

        Task<ServiceResponse<LoginResponse>> RefreshToken(string refreshToken);

        Task<ApiResponse> ForgotPassword(string email);

        Task<ApiResponse> ChangePassword(string currentEmailId, ChangePasswordModel model);

        Task<ApiResponse> ResetPassword(string email, string token, string password);

        Task<ApiResponse> VerifyEmail(string token);

        Task<ApiResponse> ResendVerifyEmail(string email);

        Task<ServiceResponse<LoginResponse>> VerifyOTP(OTPModel model);

        Task<ServiceResponse<LoginOtpResponse>> ResendVerifyOTP(ResendOTPModel model);


    }
}
