using Document_Blockchain.Constants;
using Document_Blockchain.IInternalServices;
using Document_Blockchain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Document_Blockchain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebUserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public WebUserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ServiceResponse<List<ViewCart>>> ViewCartItems()
        {
            var result = await _unitOfWork.WebUserServices.ViewCart();
            return result;
        }

        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> AddToCart(int documentId)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var result = await _unitOfWork.WebUserServices.AddToCart(documentId);
            return result;
        }

        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> RemoveFromCart(int documentId)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var result = await _unitOfWork.WebUserServices.RemoveFromCart(documentId);
            return result;
        }

        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ServiceResponse<List<ViewDocument>>> UserDocuments(bool documentId, int pastWeek)
        {
            var result = await _unitOfWork.WebUserServices.UserDocuments(documentId, pastWeek);
            return result;
        }


        [Authorize(Roles = "User")]
        [HttpGet("[action]")]
        public async Task<ServiceResponse<ViewDocument>> ViewDocumentById(long Id)
        {
            var signstatus = await _unitOfWork.Docusign.GetSignatureStatus(Id);
            if (!ModelState.IsValid)
                return new ServiceResponse<ViewDocument>(400, ApiResponseConstants.invalidInput, null);

            var result = await _unitOfWork.WebUserServices.ViewDocumentById(Id);
            return result;
        }


        [Authorize(Roles = "User")]
        [HttpGet("[action]")]
        public async Task<ServiceResponse<List<EnvelopeDetailsResponse>>> UserSentDocuments()
        {
            var result = await _unitOfWork.WebUserServices.UserSentDocuments();
            return result;
        }


        [Authorize(Roles = "User")]
        [HttpGet("[action]")]
        public async Task<ServiceResponse<EnvelopeDetailsResponse>> UserSentDocumentById(string envId)
        {
            if (!ModelState.IsValid)
                return new ServiceResponse<EnvelopeDetailsResponse>(400, ApiResponseConstants.invalidInput, null);

            var result = await _unitOfWork.WebUserServices.UserSentDocumentById(envId);
            return result;
        }


        [Authorize(Roles = "User,Admin")]
        [HttpGet("[action]")]
        public async Task<ServiceResponse<ProfileViewModel>> ViewProfile()
        {
            var result = await _unitOfWork.WebUserServices.ViewProfile();
            return result;
        }

        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> UpdateProfile(ProfileUpdateModel model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var res = await _unitOfWork.WebUserServices.UpdateProfile(model);
            return res;
        }


        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<List<CountryModel>> GetCountry()
        {
            var query = await _unitOfWork.WebUserServices.GetCountry();
            return query;
        }


        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> UploadEditedDocument(AddEditedDocument model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, StatusCodes.Status400BadRequest);

            var result = await _unitOfWork.WebUserServices.UploadEditedDocument(model);
            return result;
        }

        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> RequestNewDocument(RequestNewDocument requestNewDocument)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, StatusCodes.Status400BadRequest);

            var result = await _unitOfWork.WebUserServices.RequestNewDocument(requestNewDocument);
            return result;
        }

        [Authorize(Roles = "User")]
        [HttpGet("[action]")]
        public async Task<ServiceResponse<List<ViewNewDocumentRequest>>> ViewMyDocumentRequests()
        {
            var result = await _unitOfWork.WebUserServices.ViewMyDocumentRequests();
            return result;
        }

        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> EditMyRequest(EditRequestNewDocument model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, StatusCodes.Status400BadRequest);

            var result = await _unitOfWork.WebUserServices.EditMyRequest(model);
            return result;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ApiResponse> SendLegalContactMail(LegalContactModel model)
        {
            var res = await _unitOfWork.WebUserServices.SendLegalContactMail(model);
            return res;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ApiResponse> ContactUsEmail(ContactUsModel model)
        {
            var res = await _unitOfWork.WebUserServices.ContactUsEmail(model);
            return res;
        }

        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> ConfirmResend(ConfirmResentModel model)
        {
            var res = await _unitOfWork.WebUserServices.ConfirmResend(model);
            return res;
        }

        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ServiceResponse<ResendDocumentModel>> ResendDocument(int DocumentId)
        {
            var res = await _unitOfWork.WebUserServices.ResendDocument(DocumentId);
            return res;
        }


    }
}
