using Document_Blockchain.Constants;
using Document_Blockchain.Entities;
using Document_Blockchain.IInternalServices;
using Document_Blockchain.Models;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Document_Blockchain.Docusign_
{


    public class Docusign : IDocusign
    {
        private readonly IConfiguration _configuration;
        private readonly IUserServices _userService;
        private readonly IStorage _storage;
        private readonly BlockChainDbContext _context;

        public Docusign(IConfiguration configuration,
                        IUserServices userService,
                        IStorage storage,
                        BlockChainDbContext context)
        {
            _configuration = configuration;
            _userService = userService;
            _storage = storage;
            _context = context;
        }

        

        private static EnvelopeDefinition MakeEnvelope(List<EmailModel> receipents, string docPdf)
        {
            EnvelopeDefinition env = new();
            env.EmailSubject = DocusignConstants.emailSubject;

            DocuSign.eSign.Model.Document doc = new()
            {
                DocumentBase64 = docPdf,
                Name = DocusignConstants.defaultDocumentName, // can be different from actual file name
                FileExtension = MediaExtentions.pdf,
                DocumentId = NumaricsAsStrings._1
            };
            env.Documents = new List<DocuSign.eSign.Model.Document> { doc };
            int RecipientId = 1;
            int RoutingOrder = 1;
            List<Signer> signers = new();
            foreach (var receipent in receipents)
            {
                Signer signer = new()
                {
                    Email = receipent.EmailAddress,
                    Name = receipent.SignerName,
                    RecipientId = RecipientId.ToString(),
                    RoutingOrder = RoutingOrder.ToString()
                };
                SignHere signHere1 = new()
                {
                    AnchorString = $"**signature_{RecipientId}**",
                    AnchorUnits = "pixels",
                    AnchorYOffset = "10",
                    AnchorXOffset = "20"
                };

                SignHere signHere2 = new()
                {
                    AnchorString = $"/sn{RecipientId}/",
                    AnchorUnits = "pixels",
                    AnchorYOffset = "10",
                    AnchorXOffset = "20"
                };
                Tabs signer1Tabs = new()
                {
                    SignHereTabs = new List<SignHere> { signHere1, signHere2 }
                };
                signer.Tabs = signer1Tabs;

                RecipientId++;
                RoutingOrder++;
                signers.Add(signer);
            }

            Recipients recipients = new()
            {
                Signers = signers,
            };
            env.Recipients = recipients;
            env.Status = EmailStatus.sent;

            return env;
        }


        public string GenerateDocusignAccessToken()
        {
            try
            {
                List<string> scopes = new()
                {
                    DocuSignScopes.impersonation,
                    DocuSignScopes.signature
                };

                var _apiClient = new ApiClient(_configuration["DocuSign:basePath"]);
                var ClientId = _configuration["DocuSignJWT:ClientId"];
                var ImpersonatedUserId = _configuration["DocuSignJWT:ImpersonatedUserId"];
                var AuthServer = _configuration["DocuSignJWT:AuthServer"];
                var privatekey = DSHelper.ReadFileContent(DSHelper.PrepareFullPrivateKeyFilePath(this._configuration["DocuSignJWT:PrivateKeyFile"]));

                var token = _apiClient.RequestJWTUserToken(
                ClientId, ImpersonatedUserId, AuthServer, privatekey, 1, scopes);
                var oldtoken = _context.DocusignAccessToken.FirstOrDefault();
                oldtoken.AccessToken = token.access_token;
                oldtoken.CreatedTime = DateTime.UtcNow;
                oldtoken.ExpaireTime = DateTime.UtcNow.AddSeconds(3000);
                oldtoken.TokenType = token.token_type;
                _context.Update(oldtoken);
                _context.SaveChanges();

                return token.access_token;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<ServiceResponse<EnvelopeSentResponse>> Sendmail(EnvelopesModel model)
        {
            var document = await _context.UserDocuments.Include(x => x.Document)
                                                      .Where(x => x.UserId == _userService.UserID &&
                                                             x.Id == model.DocumnetId)
                                                      .OrderByDescending(x => x.OrderDate)
                                                      .FirstOrDefaultAsync();
            var accesstoken = await _context.DocusignAccessToken.Where(x => x.ExpaireTime >= DateTime.UtcNow)
                                                                .Select(x => x.AccessToken)
                                                                .FirstOrDefaultAsync();
            string maildocument = "";

            List<EmailModel> newSigners = new List<EmailModel>();
            var signedDocument = await _context.EnvolopeDetails.Where(x => x.UserDocumentId == model.DocumnetId && x.SentBy == _userService.UserID).FirstOrDefaultAsync();
            try
            {
                EnvelopeSentResponse response = new();

                if (document != null)
                {
                    if (accesstoken == null)
                    {
                        accesstoken = GenerateDocusignAccessToken();

                    }

                    var _apiClient = new ApiClient(_configuration["DocuSign:basePath"]);
                    if (signedDocument == null)
                    {
                        if (document.IsEdited == true)
                        {
                            maildocument = document.EditedDocument;
                            newSigners = model.Recepients;
                        }
                        else
                        {
                            maildocument = document.Document.DocumentPath;
                            newSigners = model.Recepients;
                        }
                    }
                    else if (signedDocument.SignedDocumentURL != null)
                    {
                        maildocument = signedDocument.SignedDocumentURL;
                        document.Document.DocumentPath = signedDocument.SignedDocumentURL;
                        var allsigners = await _context.DocumentSignerDetails.Where(x => x.EnvelopeId == signedDocument.Id).ToListAsync();
                        var abc = model.Recepients.Select(x => x.EmailAddress).ToList();
                        foreach (var signer in allsigners)
                        {
                            foreach (var a in abc)
                            {
                                if (a != signer.SignerEmail)
                                {
                                    newSigners.Add(model.Recepients.Where(x => x.EmailAddress == a).FirstOrDefault());
                                }
                            }
                        }
                    }
                    else if (signedDocument.SignedDocumentURL == null)
                    {
                        return new ServiceResponse<EnvelopeSentResponse>(StatusCodes.Status404NotFound, ApiResponseConstants.notAllSignersSigned, null);
                    }
                    string newdoc64;
                    using (WebClient client = new())
                    {
                        var content = client.DownloadData(maildocument);
                        using var stream = new MemoryStream(content);
                        var filebytes = stream.ToArray();
                        newdoc64 = Convert.ToBase64String(filebytes);
                    }
                    if (newSigners.Count == 0)
                        return new ServiceResponse<EnvelopeSentResponse>(StatusCodes.Status400BadRequest,ApiResponseConstants.mailAlreadySent, null);

                    EnvelopeDefinition env = new EnvelopeDefinition();
                    if (newSigners.Count != 0)
                    {
                        env = MakeEnvelope(newSigners, newdoc64);
                    }

                    _apiClient.Configuration.AccessToken = accesstoken;
                    EnvelopesApi envelopesApi = new(_apiClient);

                    var accountid = _configuration["DocuSign:accountId"];

                    var results = envelopesApi.CreateEnvelope(accountid, env);

                    if (results.ErrorDetails != null)
                    {

                        return new ServiceResponse<EnvelopeSentResponse>(400, results.ErrorDetails.Message, null);
                    }


                    EnvolopeDetails newEnv = new()
                    {
                        EnvelopeId = results.EnvelopeId,
                        SentBy = _userService.UserID,
                        MailStatus = results.Status,
                        SentTime = DateTime.UtcNow,
                        UserDocumentId = model.DocumnetId,
                        DocumentId = document.DocumentId
                    };
                    await _context.AddAsync(newEnv);
                    await _context.SaveChangesAsync();
                    List<DocumentSignerDetails> signers = new();
                    var routing = 1;
                    foreach (var signer in model.Recepients)
                    {
                        DocumentSignerDetails newsigner = new()
                        {
                            EnvelopeId = _context.EnvolopeDetails.Where(x => x.EnvelopeId == results.EnvelopeId).Select(x => x.Id).FirstOrDefault(),
                            SignerEmail = signer.EmailAddress,
                            SignerName = signer.SignerName,
                            SignerRouting = routing,
                            SignatureStatus = EmailStatus.pending.ToLower()
                        };
                        signers.Add(newsigner);
                        routing++;
                    }
                    _context.AddRange(signers);
                    await _context.SaveChangesAsync();


                    response.EnvelopeId = results.EnvelopeId;
                    response.MailStatus = results.Status;
                    response.MailSentTime = DateTime.UtcNow;
                    response.DocumentURl = results.Uri;


                }
                return new ServiceResponse<EnvelopeSentResponse>(200, ApiResponseConstants.success, response);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EnvelopeSentResponse>(400, ex.Message, null);
            }
        }

        public async Task<ApiResponse> GetSignatureStatus(long documentId)
        {
            try
            {
                string messege = "";
                var env = await _context.EnvolopeDetails.Where(x => x.UserDocumentId == documentId && x.SentBy == _userService.UserID && x.MailStatus != "completed").ToListAsync();
                //var signers = await _context.DocumentSignerDetails.Include(x => x.EnvolopeDetails).Where(x => env.Select(x => x.EnvelopeId).Contains(x.EnvolopeDetails.EnvelopeId)).ToListAsync();
                var accesstoken = await _context.DocusignAccessToken.Where(x => x.ExpaireTime >= DateTime.UtcNow).Select(x => x.AccessToken).FirstOrDefaultAsync();
                if (env.Count == 0)
                {
                    return new ApiResponse(ApiResponseConstants.envelopeDoesNotExists, 400);
                }
                var _apiClient = new ApiClient(_configuration["DocuSign:basePath"]);
                if (accesstoken == null)
                {
                    accesstoken = GenerateDocusignAccessToken();
                }
                _apiClient.Configuration.AccessToken = accesstoken;
                var accountid = _configuration["DocuSign:accountId"];
                EnvelopesApi envelopesApi = new(_apiClient);
                foreach (var abc in env)
                {
                    var signers = await _context.DocumentSignerDetails.Include(x => x.EnvolopeDetails)
                                                                     .Where(x => x.EnvelopeId == abc.Id)
                                                                     .ToListAsync();
                    var envelopeStatus = envelopesApi.GetEnvelope(accountid, abc.EnvelopeId);
                    if (envelopeStatus.Status != ApiResponseConstants.completed)
                    {
                        try
                        {
                            foreach (var receipent in signers)
                            {
                                var documentStram1 = envelopesApi.GetRecipientSignature(accountid, abc.EnvelopeId, receipent.SignerRouting.ToString());
                                if (documentStram1 != null)
                                {
                                    receipent.SignatureStatus = DocusignConstants.Signed;
                                    receipent.SignatureTime = documentStram1.AdoptedDateTime;
                                    _context.Update(receipent);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        catch (Exception)
                        {
                            foreach (var receipent in signers)
                            {
                                if (receipent.SignatureStatus != DocusignConstants.Signed)
                                {
                                    messege = messege + receipent.SignerName + DocusignConstants.NotSigned;
                                }
                            }
                        }
                    }
                    else if (envelopeStatus.Status == ApiResponseConstants.completed)
                    {
                        abc.MailStatus = ApiResponseConstants.completed;
                        abc.SentTime = DateTime.Now;
                        if (abc.SignedDocumentURL == null)
                        {
                            var documentStram = envelopesApi.GetDocument(accountid, abc.EnvelopeId, NumaricsAsStrings._1);

                            MemoryStream ms;
                            ms = new MemoryStream();
                            documentStram.CopyTo(ms);

                            var docUpload = await _storage.UploadFile($"substorage/Documentstorage", documentStram, Guid.NewGuid().ToString() + MediaExtentions.pdf);

                            var docUrl = docUpload.Data;
                            abc.SignedDocumentURL = docUrl;
                        }
                        foreach (var receipent in signers)
                        {
                            if (receipent.SignatureStatus != DocusignConstants.Signed)
                            {
                                var documentStram1 = envelopesApi.GetRecipientSignature(accountid, abc.EnvelopeId, receipent.SignerRouting.ToString());
                                if (documentStram1 != null)
                                {
                                    receipent.SignatureStatus = DocusignConstants.Signed;
                                    receipent.SignatureTime = documentStram1.AdoptedDateTime;
                                    _context.Update(receipent);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        _context.Update(abc);
                        await _context.SaveChangesAsync();
                    }
                }
                if (messege == "")
                {
                    return new ApiResponse(DocusignConstants.AllSigned, 200);
                }
                else
                {
                    return new ApiResponse(messege, StatusCodes.Status400BadRequest);

                }
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message, 500);
            }
        }

        public async Task<ApiResponse<SignedDocumentResponse>> SignedDocumentSubmission(string envelopeId)
        {
            var details = await _context.EnvolopeDetails.Include(x => x.DocumentSignerDetails)
                                                       .Include(x => x.User)
                                                       .Where(x => x.EnvelopeId == envelopeId)
                                                       .Select(x => new SignedDocumentResponse
                                                       {
                                                           SignatureRequestedByEmail = x.User.EmailId,
                                                           SignatureRequestedByName = x.User.FullName,
                                                           SignedDocumentURL = x.SignedDocumentURL,
                                                           SignedUsers = x.DocumentSignerDetails.Select(x => new EmailModel
                                                           {
                                                               EmailAddress = x.SignerEmail,
                                                               SignerName = x.SignerName
                                                           }).ToList()
                                                       }).FirstOrDefaultAsync();
            if (details == null)
            {
                return new ApiResponse<SignedDocumentResponse>(400, ApiResponseConstants.envelopeDoesNotExists, null);
            }
            else if (details != null && details.SignedDocumentURL == null)
            {
                return new ApiResponse<SignedDocumentResponse>(400, ApiResponseConstants.notAllSignersSigned, null);
            }
            else
            {
                return new ApiResponse<SignedDocumentResponse>(200, ApiResponseConstants.success, details);
            }
        }


        public async Task<ApiResponse> AddHashDetails(AddHashDetailsModel model)
        {
            var checkitem = await _context.HashDetails.Where(x => x.UserDocumentId == model.DocumentId &&
                                                                  x.UserId == _userService.UserID)
                                                      .FirstOrDefaultAsync();


            if (checkitem != null)
            {
                return new ApiResponse(ApiResponseConstants.alreadyUploaded, 400);
            }

            HashDetails newItem = new()
            {
                UserId = _userService.UserID,
                Hash = model.Hash,
                DocumentId = _context.UserDocuments.Where(x => x.Id == model.DocumentId).Select(x => x.DocumentId).FirstOrDefault(),
                UserDocumentId = model.DocumentId,
                IPFS = model.IPFS,
                BlockchainId = model.BlockchainId,
                BlockchainTrx = model.BlockchainTrx,
                Status = model.Status,
                CreatedDate = System.DateTime.UtcNow
            };
            await _context.AddAsync(newItem);
            await _context.SaveChangesAsync();
            return new ApiResponse(ApiResponseConstants.success, 200);
        }



        public async Task<ApiResponse> BlockchainUpload(UploadBlockchainModel uploadmodel)
        {
            var hashdetails = _context.HashDetails.Where(x => x.UserDocumentId == uploadmodel.Id &&
                                                             x.UserId == _userService.UserID)
                                                  .FirstOrDefault();
            if (hashdetails != null)
                return new ApiResponse(ApiResponseConstants.alreadyUploaded, StatusCodes.Status208AlreadyReported);

            using (var client = new HttpClient())
            {
                CallApiModel callApiModel = new();
                var requestUrl = _configuration["DocuSign:blockchain_url"];
                callApiModel.path = uploadmodel.PathFromS3;

                var objAsJson = JsonConvert.SerializeObject(callApiModel);

                var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUrl, content).ConfigureAwait(false);

                var responseContent = response.Content.ReadAsStringAsync().Result;

                ResponseModel responseFromAPI = JsonConvert.DeserializeObject<ResponseModel>(responseContent);
                AddHashDetailsModel model = new AddHashDetailsModel()
                {
                    Hash = responseFromAPI.hash,
                    BlockchainId = responseFromAPI.blockchainId,
                    BlockchainTrx = responseFromAPI.blockchainTrx,
                    DocumentId = uploadmodel.Id,
                    IPFS = responseFromAPI.ipfs,
                    Status = responseFromAPI.status
                };
                await AddHashDetails(model);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new ApiResponse(ApiResponseConstants.hashGenerated, StatusCodes.Status200OK);
                }
                else
                {
                    return new ApiResponse(ApiResponseConstants.hashNotGenerated, StatusCodes.Status400BadRequest);

                }


            }
        }
    }

}
