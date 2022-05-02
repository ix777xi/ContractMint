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
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ServiceResponse<List<Categories>>> ViewAllCategories(ViewAllCategoriesModel model)
        {
            var result = await _unitOfWork.AdminServices.ViewAllCategories(model);
            return result;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public async Task<ServiceResponse<List<ViewDocumentRequest>>> ViewDocumentRequests()
        {
            var result = await _unitOfWork.AdminServices.ViewDocumentRequests();
            return result;
        }


        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<ServiceResponse<List<ViewAdminDocument>>> ViewAllDocuments()
        {
            var result = await _unitOfWork.AdminServices.ViewAllDocuments();
            return result;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> EditDocumentRequestStatus(long id, long status)
        {
            var result = await _unitOfWork.AdminServices.EditDocumentRequestStatus(id, status);
            return result;
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<ServiceResponse<List<StatusDropDownModel>>> RequestStatusDropDown()
        {
            var result = await _unitOfWork.AdminServices.RequestStatusDropDown();
            return result;
        }



        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ServiceResponse<List<ViewDocument>>> ViewAllDocumentsByCat(ViewDocumentsByCategoryModel model)
        {
            if (!ModelState.IsValid)
                return new ServiceResponse<List<ViewDocument>>(400, ApiResponseConstants.invalidInput, null);

            var result = await _unitOfWork.AdminServices.ViewAllDocumentsByCat(model);
            return result;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> AddNewDocument(NewDocument model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var result = await _unitOfWork.AdminServices.AddNewDocument(model);
            return result;
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("[action]")]
        public async Task<ApiResponse> UpdateDocument(UpdateDocument model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var result = await _unitOfWork.AdminServices.UpdateDocument(model);
            return result;
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]")]
        public async Task<ApiResponse> DeleteDocument(int Id)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var result = await _unitOfWork.AdminServices.DeleteDocument(Id);
            return result;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> AddCategory(AddCatModel model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);
            var result = await _unitOfWork.AdminServices.AddCategory(model);
            return result;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> UpdateCategory(EditCatModel model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, StatusCodes.Status400BadRequest);

            var result = await _unitOfWork.AdminServices.UpdateCategory(model);
            return result;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]")]
        public async Task<ApiResponse> DeleteCategory(long categoryId)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, StatusCodes.Status400BadRequest);

            var result = await _unitOfWork.AdminServices.DeleteCategory(categoryId);
            return result;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public async Task<ServiceResponse<List<ProfileViewModel>>> AllRegisteredUsers()
        {
            var result = await _unitOfWork.AdminServices.AllUsers();
            return result;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public async Task<ServiceResponse<List<PaymentLedgerResponse>>> AllPaymentDetails()
        {
            var result = await _unitOfWork.AdminServices.AllPaymentDetails();
            return result;
        }


        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> BlockchainUpload(UploadBlockchainModel uploadmodel)
        {
            var result = await _unitOfWork.Docusign.BlockchainUpload(uploadmodel);
            return result;
        }
    }
}
