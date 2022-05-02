using Document_Blockchain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Document_Blockchain.IInternalServices
{
    public interface IWebUserServices
    {
        Task<ApiResponse> AddToCart(int documentId);

        Task<ApiResponse> RemoveFromCart(int documentId);

        Task<ServiceResponse<List<ViewCart>>> ViewCart();

        Task<ServiceResponse<ViewDocument>> ViewDocumentById(long Id);

        Task<ServiceResponse<List<ViewDocument>>> UserDocuments(bool isRecent, int pastWeek);

        Task<ServiceResponse<List<EnvelopeDetailsResponse>>> UserSentDocuments();

        Task<ServiceResponse<EnvelopeDetailsResponse>> UserSentDocumentById(string envId);

        Task<ServiceResponse<ProfileViewModel>> ViewProfile();

        Task<ApiResponse> UpdateProfile(ProfileUpdateModel model);

        Task<ApiResponse> UploadEditedDocument(AddEditedDocument model);

        Task<ApiResponse> RequestNewDocument(RequestNewDocument requestNewDocument);

        Task<ServiceResponse<List<ViewNewDocumentRequest>>> ViewMyDocumentRequests();

        Task<ApiResponse> EditMyRequest(EditRequestNewDocument model);

        Task<ApiResponse> SendLegalContactMail(LegalContactModel model);

        Task<ApiResponse> ContactUsEmail(ContactUsModel model);

        Task<List<CountryModel>> GetCountry();

        Task<ServiceResponse<ResendDocumentModel>> ResendDocument(int DocumentId);

        Task<ApiResponse> ConfirmResend(ConfirmResentModel model);

    }
}
