using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Document_Blockchain.IInternalServices;
using Document_Blockchain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Document_Blockchain.InternalService
{


    public class Storage : IStorage
    {
        private readonly IConfiguration _configuration;

        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSouth1;

        //private string ConnectionString => _configuration.GetValue<string>("Azure:BlobConnectionString");

        public Storage(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static int MAX_FILE_SIZE = 1024 * 1024 * 20;// 20 MB
        public static string[] EXTENSION_LOWER_CASE = new string[] { "pdf", "jpg", "png", "html", "csv" };

        public string BaseUrl => $"https://{_configuration.GetValue<string>("AWS:bucketName")}.s3.ap-south-1.amazonaws.com/";

        public bool ValidateFilesSize(IFormFile file)
        {
            return (file == null) ? false : (file.Length > MAX_FILE_SIZE) ? false : true;
        }
        public string GetFileExtension(IFormFile file)
        {
            return (file == null) ? "" : Path.GetExtension(file.FileName).ToLower();
        }
        public bool IsExtensionsAvailable(string extension)
        {
            return EXTENSION_LOWER_CASE.Contains(extension) ? false : true;
        }

        public string GenerateS3Path(string path, string fileName)
        {
            var s = $"{path}/{Guid.NewGuid()}{Path.GetExtension(fileName).ToLower()}";
            return s;
        }


        public async Task<ServiceResponse<DeleteObjectResponse>> DeleteObject(string filepath)
        {
            try
            {
                using AmazonS3Client client = new(_configuration.GetValue<string>("AWS:accessKey"), _configuration.GetValue<string>("AWS:secretKey"), bucketRegion);
                DeleteObjectRequest request = new()
                {
                    BucketName = _configuration.GetValue<string>("AWS:bucketName"),
                    Key = filepath.Remove(0, BaseUrl.Length - 1)
                };

                var response = await client.DeleteObjectAsync(request);
                return new ServiceResponse<DeleteObjectResponse>(StatusCodes.Status200OK, "Success", response);
            }
            catch (AmazonS3Exception e)
            {
                return new ServiceResponse<DeleteObjectResponse>(StatusCodes.Status500InternalServerError, e.Message, null);
            }
            catch (Exception e)
            {
                return new ServiceResponse<DeleteObjectResponse>(StatusCodes.Status500InternalServerError, e.Message, null);
            }
        }

        public async Task<ServiceResponse<string>> UploadFile(string path, IFormFile file)
        {
            try
            {
                using IAmazonS3 client = new AmazonS3Client(_configuration.GetValue<string>("AWS:accessKey"), _configuration.GetValue<string>("AWS:secretKey"), bucketRegion);
                if (file == null)
                    return new ServiceResponse<string>(StatusCodes.Status400BadRequest, "File was null", null);

                if (ValidateFilesSize(file) == false)
                    return new ServiceResponse<string>(StatusCodes.Status413RequestEntityTooLarge, "Entity Too Large", null);

                //if (IsExtensionsAvailable(file.FileName.Split(".")[1]))
                if (IsExtensionsAvailable(file.FileName.Split(".")[file.FileName.Split(".").GetUpperBound(0)]))

                    return new ServiceResponse<string>(StatusCodes.Status406NotAcceptable, "Upload correct file format", null);

                var filetransferutility = new TransferUtility(client);
                var filePath = GenerateS3Path(path, file.FileName);
                using (var fileStream = file.OpenReadStream())
                {
                    await filetransferutility.UploadAsync(fileStream, _configuration.GetValue<string>("AWS:bucketName"), filePath);
                }
                var result = await client.PutACLAsync(new Amazon.S3.Model.PutACLRequest
                {
                    BucketName = _configuration.GetValue<string>("AWS:bucketName"),
                    CannedACL = S3CannedACL.PublicRead,
                    Key = filePath
                });

                if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    return new ServiceResponse<string>(StatusCodes.Status200OK, "", $"{BaseUrl}{filePath}");

                else
                    return new ServiceResponse<string>(StatusCodes.Status400BadRequest, "", $"{BaseUrl}{filePath}");

            }
            catch (AmazonS3Exception e)
            {
                return new ServiceResponse<string>(StatusCodes.Status500InternalServerError, e.Message, null);
            }
            catch (Exception e)
            {
                return new ServiceResponse<string>(StatusCodes.Status500InternalServerError, e.Message, null);
            }
        }

        public async Task<Stream> GetFile(string path)
        {
            using IAmazonS3 client = new AmazonS3Client(_configuration.GetValue<string>("AWS:accessKey"), _configuration.GetValue<string>("AWS:secretKey"), bucketRegion);
            var filetransferutility = new TransferUtility(client);
            path = path.Remove(0, BaseUrl.Length);
            var stream = await filetransferutility.OpenStreamAsync(_configuration.GetValue<string>("AWS:bucketName"), path);
            return stream;
        }

        //For type :- Base64 files
        public async Task<ServiceResponse<string>> UploadFile(string path, string file, string extension)
        {
            try
            {
                byte[] inBytes = Convert.FromBase64String(file);
                var stream = new MemoryStream(inBytes);
                Random random = new();
                var fileName = random.Next(100000, 199999).ToString();

                using IAmazonS3 client = new AmazonS3Client
                    (_configuration.GetValue<string>("AWS:accessKey"), _configuration.GetValue<string>("AWS:secretKey"), bucketRegion);
                var filetransferutility = new TransferUtility(client);
                var filePath = GenerateS3Path(path, $"{fileName}{extension}");
                await filetransferutility.UploadAsync(stream, _configuration.GetValue<string>("AWS:bucketName"), filePath);
                await client.PutACLAsync(new PutACLRequest
                {
                    BucketName = _configuration.GetValue<string>("AWS:bucketName"),
                    CannedACL = S3CannedACL.PublicRead,
                    Key = filePath
                });
                return new ServiceResponse<string>(StatusCodes.Status200OK, "", $"{BaseUrl}{filePath}");
            }
            catch (AmazonS3Exception e)
            {
                return new ServiceResponse<string>(StatusCodes.Status500InternalServerError, e.Message, null);
            }
            catch (Exception e)
            {
                return new ServiceResponse<string>(StatusCodes.Status500InternalServerError, e.Message, null);
            }
        }

        public async Task<ServiceResponse<string>> UploadFile(string path, MemoryStream stream, string fileName)
        {
            try
            {
                using IAmazonS3 client = new AmazonS3Client(_configuration.GetValue<string>("AWS:accessKey"), _configuration.GetValue<string>("AWS:secretKey"), bucketRegion);
                var filetransferutility = new TransferUtility(client);
                var filePath = GenerateS3Path(path, fileName);
                await filetransferutility.UploadAsync(stream, _configuration.GetValue<string>("AWS:bucketName"), filePath);
                await client.PutACLAsync(new PutACLRequest
                {
                    BucketName = _configuration.GetValue<string>("AWS:bucketName"),
                    CannedACL = S3CannedACL.PublicRead,
                    Key = filePath
                });
                return new ServiceResponse<string>(StatusCodes.Status200OK, "", $"{BaseUrl}{filePath}");
            }
            catch (AmazonS3Exception e)
            {
                return new ServiceResponse<string>(StatusCodes.Status500InternalServerError, e.Message, null);
            }
            catch (Exception e)
            {
                return new ServiceResponse<string>(StatusCodes.Status500InternalServerError, e.Message, null);
            }
        }


        public async Task<ApiResponse> DeleteFile(string path)
        {
            if (path is null)
                return new ApiResponse("Path cannot be null", 400);

            var keyFileName = string.Empty;
            var splitPath = path.Split("/");
            for (int i = 0; i < splitPath.Length; i++)
            {
                if (splitPath[i].EndsWith(".jpeg") || splitPath[i].EndsWith(".pdf") || splitPath[i].EndsWith(".png"))
                    keyFileName = $"{_configuration.GetValue<string>("AWS:subFolder")}/{splitPath[i]}";
            }
            using IAmazonS3 client = new AmazonS3Client(_configuration.GetValue<string>("AWS:accessKey"),
                _configuration.GetValue<string>("AWS:secretKey"), bucketRegion);
            DeleteObjectRequest request = new()
            {
                BucketName = _configuration.GetValue<string>("AWS:bucketName"),
                Key = keyFileName
            };
            var abc = await client.DeleteObjectAsync(request);
            return new ApiResponse("Successfully Removed", 200);
        }





        public async Task<ServiceResponse<string>> UploadFile(string path, Stream stream, string fileName)
        {
            try
            {
                using IAmazonS3 client = new AmazonS3Client(_configuration.GetValue<string>("AWS:accessKey"), _configuration.GetValue<string>("AWS:secretKey"), bucketRegion);
                var filetransferutility = new TransferUtility(client);
                var filePath = GenerateS3Path(path, fileName);
                await filetransferutility.UploadAsync(stream, _configuration.GetValue<string>("AWS:bucketName"), filePath);
                await client.PutACLAsync(new PutACLRequest
                {
                    BucketName = _configuration.GetValue<string>("AWS:bucketName"),
                    CannedACL = S3CannedACL.PublicRead,
                    Key = filePath
                });
                return new ServiceResponse<string>(StatusCodes.Status200OK, "", $"{BaseUrl}{filePath}");
            }
            catch (AmazonS3Exception e)
            {
                return new ServiceResponse<string>(StatusCodes.Status500InternalServerError, e.Message, null);
            }
            catch (Exception e)
            {
                return new ServiceResponse<string>(StatusCodes.Status500InternalServerError, e.Message, null);
            }
        }
    }
}
