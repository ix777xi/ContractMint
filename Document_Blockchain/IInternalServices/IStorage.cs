using Amazon.S3.Model;
using Document_Blockchain.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Document_Blockchain.IInternalServices
{
    public interface IStorage
    {
        public bool ValidateFilesSize(IFormFile file);
        public bool IsExtensionsAvailable(string extension);
        public string GenerateS3Path(string path, string fileName);
        public string GetFileExtension(IFormFile file);
        public Task<ServiceResponse<string>> UploadFile(string path, IFormFile file);
        public Task<ServiceResponse<string>> UploadFile(string path, MemoryStream stream, string fileName);

        Task<ServiceResponse<string>> UploadFile(string path, string file, string extension);
        public Task<Stream> GetFile(string path);
        public string BaseUrl { get; }
        Task<ApiResponse> DeleteFile(string path);

        Task<ServiceResponse<string>> UploadFile(string path, Stream stream, string fileName);


        public Task<ServiceResponse<DeleteObjectResponse>> DeleteObject(string filepath);

    }
}
