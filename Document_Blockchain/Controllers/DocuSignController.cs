using Document_Blockchain.Constants;
using Document_Blockchain.IInternalServices;
using Document_Blockchain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Document_Blockchain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocuSignController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DocuSignController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [Authorize]
        [HttpPost("[action]")]
        public async Task<ServiceResponse<EnvelopeSentResponse>> SendEmail(EnvelopesModel model)
        {
            if (!ModelState.IsValid)
                return new ServiceResponse<EnvelopeSentResponse>(400, ApiResponseConstants.invalidInput, null);

            var result = await _unitOfWork.Docusign.Sendmail(model);
            return result;
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<ApiResponse> GetSignatureStatus(long documentId)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var result = await _unitOfWork.Docusign.GetSignatureStatus(documentId);
            return result;

        }

        [Authorize(Roles = "User")]
        [HttpGet("[action]")]
        public async Task<ApiResponse<SignedDocumentResponse>> SignedDocumentSubmission(string envelopeId)
        {
            if (!ModelState.IsValid)
                return new ApiResponse<SignedDocumentResponse>(400, ApiResponseConstants.invalidInput, null);
            var results = await _unitOfWork.Docusign.SignedDocumentSubmission(envelopeId);
            return results;
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<ApiResponse> AddHashDetails(AddHashDetailsModel model)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);

            var result = await _unitOfWork.Docusign.AddHashDetails(model);
            return result;

        }


        [AllowAnonymous]
        [HttpPost("[action]")]
        public string GenerateDocusignAccessToken()
        {
            var result = _unitOfWork.Docusign.GenerateDocusignAccessToken();
            return result;
        }
    }
}
