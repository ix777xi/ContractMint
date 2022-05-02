using Document_Blockchain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Document_Blockchain.IInternalServices
{
    public interface IEmailServices
    {
        Task<ApiResponse> ForgotPasswordEmail(string email, string url);

        Task<ApiResponse> StatusChangeMail(string email, string url);


        Task<ApiResponse> VerifyRegistrationEmail(string name, string email, string url);

        Task<ApiResponse> LegalContactMail(LegalContactModel model);

        Task<ApiResponse> NewRequest(NewRequest model);


        Task<ApiResponse> ContactUsEmail(ContactUsModel model);

        Task<ApiResponse> ResendVerifyOTPMail(string email, string OTP);


        public Task SendMail(string subject, string email, string content, List<string> attachments = null);

    }
}
