using Document_Blockchain.Models;
using System.Threading.Tasks;

namespace Document_Blockchain.Docusign_
{
    public interface IDocusign
    {
        Task<ServiceResponse<EnvelopeSentResponse>> Sendmail(EnvelopesModel model);

        Task<ApiResponse> GetSignatureStatus(long documentId);

        Task<ApiResponse<SignedDocumentResponse>> SignedDocumentSubmission(string envelopeId);

        Task<ApiResponse> AddHashDetails(AddHashDetailsModel model);

        public string GenerateDocusignAccessToken();

        Task<ApiResponse> BlockchainUpload(UploadBlockchainModel uploadmodel);

    }
}
