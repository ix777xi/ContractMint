using Document_Blockchain.Entities;
using Document_Blockchain.IInternalServices;
using Document_Blockchain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Document_Blockchain.InternalService
{



    public class WebUserServices : IWebUserServices
    {
        private readonly IUserServices _userService;
        private readonly IStorage _storage;
        private readonly IEmailServices _emailService;
        private readonly BlockChainDbContext _context;

        public WebUserServices(IUserServices userService,
                               IStorage storage,
                               IEmailServices emailService,
                               BlockChainDbContext context)
        {
            this._userService = userService;
            this._storage = storage;
            this._emailService = emailService;
            this._context = context;
        }

        public async Task<ApiResponse> AddToCart(int documentId)
        {
            var checkitem = await _context.UserCart.Where(x => x.UserId == _userService.UserID &&
                                                              x.DocumentId == documentId).FirstOrDefaultAsync();
            var userDoc = await _context.UserDocuments.Where(x => x.UserId == _userService.UserID &&
                                                                 x.DocumentId == documentId).FirstOrDefaultAsync();
            if (checkitem != null)
            {
                //checkitem.Quantity += quantity;
                //context.Update(checkitem);
                //await context.SaveChangesAsync();
                return new ApiResponse("Item already in cart", 400);
            }
            if (userDoc != null)
            {
                return new ApiResponse("Item already Purchased", 400);
            }

            UserCart newItem = new()
            {
                UserId = _userService.UserID,
                DocumentId = documentId,
                Quantity = 1,
                CreatedDate = System.DateTime.UtcNow
            };
            await _context.AddAsync(newItem);
            await _context.SaveChangesAsync();
            return new ApiResponse("Contract Added to Cart", 200);
        }

        public async Task<ApiResponse> EditMyRequest(EditRequestNewDocument model)
        {
            var CurrentUser = _userService.UserID;
            var edit = await _context.DocumentRequest.Where(x => x.UserId == CurrentUser && x.Id == model.Id).FirstOrDefaultAsync();
            if (model.NewDocumentDescription != null && edit.Status == 1)
            {
                edit.DocumentDescription = model.NewDocumentDescription;
                edit.UpdatedDate = DateTime.UtcNow;
                edit.CategoryId = model.CategoryId;
            }
            else if (edit.Status != 1)
            {
                return new ApiResponse("Admin already accepted your request you cant edit now", StatusCodes.Status400BadRequest);
            }
            _context.Update(edit);
            await _context.SaveChangesAsync();
            return new ApiResponse("request updated", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse> RemoveFromCart(int documentId)
        {
            var checkitem = await _context.UserCart.Where(x => x.UserId == _userService.UserID &&
                                                              x.DocumentId == documentId &&
                                                              x.OrderStatus != "succeeded").FirstOrDefaultAsync();
            var checkitemordered = await _context.UserCart.Where(x => x.UserId == _userService.UserID &&
                                                              x.OrderStatus == "succeeded").ToListAsync();

            if (checkitem == null && checkitemordered != null)
            {
                _context.RemoveRange(checkitemordered);
                await _context.SaveChangesAsync();
                return new ApiResponse("No Items Available In Cart", 400);
            }
            else if (checkitem != null && checkitemordered != null)
            {
                _context.RemoveRange(checkitemordered);
                _context.Remove(checkitem);
                await _context.SaveChangesAsync();
                return new ApiResponse("Removed Successfully From Cart", 200);
            }
            else
            {
                _context.Remove(checkitem);
                await _context.SaveChangesAsync();
                return new ApiResponse("Removed Successfully From Cart", 200);
            }

            //var getdocumentstatus = await context.UserOrders.Where(x => x.UserId == userService.UserID)
            //                                                .Select(x => x.OrderSatus)
            //                                                .FirstOrDefaultAsync();

            //if (checkitem != null && checkitem.Quantity == quantity || quantity == 0 && getdocumentstatus == "succeeded")
            //{
            //    context.Remove(checkitem);
            //    await context.SaveChangesAsync();
            //    return new ApiResponse("removed successfully", 200);
            //}
            //else if (checkitem != null && checkitem.Quantity > quantity && getdocumentstatus != "succeeded")
            //{
            //    checkitem.Quantity -= quantity;
            //    checkitem.OrderStatus = getdocumentstatus;
            //    context.Update(checkitem);
            //    await context.SaveChangesAsync();
            //    return new ApiResponse("removed successfully", 200);
            //}
            //else
            //{
            //    return new ApiResponse("invalid operation", 400);
            //}
        }

        public async Task<ApiResponse> RequestNewDocument(RequestNewDocument requestNewDocument)
        {
            var CurrentUser = _userService.UserID;
            DocumentRequest NewRequest = new()
            {
                UserId = CurrentUser,
                DocumentDescription = requestNewDocument.NewDocumentDescription,
                CategoryId = requestNewDocument.Category,
                Status = 1,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            var user = _context.User.Where(x => x.Id == CurrentUser).FirstOrDefault();
            var request = new NewRequest()
            {
                Name = user.FullName,
                Email = user.EmailId,
                PhoneNumber = user.ContactNumber,
                Description = requestNewDocument.NewDocumentDescription
            };

            await _context.AddAsync(NewRequest);
            await _context.SaveChangesAsync();

            await _emailService.NewRequest(request);

            return new ApiResponse("Request Sent Successfully", StatusCodes.Status200OK);
        }



        public async Task<ApiResponse> UpdateProfile(ProfileUpdateModel model)
        {
            var currentUser = _userService.UserID;
            var userdetails = await _context.User.Where(x => x.Id == currentUser).FirstOrDefaultAsync();
            var countries = await _context.Country.Select(x => x.Id).ToListAsync();
            if (userdetails != null)
            {
                userdetails.FullName = model.UserName;
                userdetails.ContactNumber = model.PhoneNumber;
                if (countries.Contains(model.Country))
                {
                    userdetails.CountryId = model.Country;
                }
                else
                {
                    return new ApiResponse("Please Select a Valid Country", StatusCodes.Status406NotAcceptable);
                }
                if (model.ProfilePhoto == null)
                {
                    if (userdetails.ProfilePicture != null)
                    {
                        var res = await _storage.DeleteFile(userdetails.ProfilePicture);
                    }
                    userdetails.ProfilePicture = null;
                }

                if (model.ProfilePhoto != null)
                {
                    var uploadPic = await _storage.UploadFile($"substorage/Documentstorage", model.ProfilePhoto, model.Extention);

                    userdetails.ProfilePicture = uploadPic.Data;
                }
                _context.Update(userdetails);
                await _context.SaveChangesAsync();
            }
            return new ApiResponse("Profile Updated Successfully", 200);
        }

        public async Task<ApiResponse> UploadEditedDocument(AddEditedDocument model)
        {
            var currentUser = _userService.UserID;
            var userdoc = await _context.UserDocuments.Where(x => x.UserId == currentUser && x.Id == model.Id).FirstOrDefaultAsync();
            if (model.EditedDocument != null)
            {
                var docPath = await _storage.UploadFile($"substorage/Documentstorage", model.EditedDocument, model.Extention);
                if (docPath.StatusCode == 200)
                {
                    userdoc.IsEdited = true;
                    userdoc.EditedDocument = docPath.Data;
                }
                else
                {
                    return new ApiResponse("upload failed", StatusCodes.Status404NotFound);
                }
                _context.Update(userdoc);
                await _context.SaveChangesAsync();
            }
            else
            {
                return new ApiResponse("please upload valid Document", StatusCodes.Status400BadRequest);
            }
            return new ApiResponse("uploaded successfully", StatusCodes.Status200OK);
        }

        public async Task<ServiceResponse<List<ViewDocument>>> UserDocuments(bool isRecent, int pastWeek)
        {
            var currentuser = _userService.UserID;
            if (!isRecent)
            {
                var documents = await _context.UserDocuments.Include(x => x.Document)
                                                           .Where(x => x.UserId == currentuser)
                                                           .ToListAsync();
                var userdocs = documents.Select(x => new ViewDocument
                {
                    Id = x.Id,
                    DocumentId = x.Document.Id,
                    DocumentName = x.Document.DocumentName,
                    Description = x.Document.Description,
                    AlternateName = x.AlternateName,
                    DocumentPath = x.Document.DocumentPath,
                    OrderDate = x.OrderDate
                }).OrderByDescending(x => x.OrderDate).ToList();
                if (userdocs.Count == 0)
                    return new ServiceResponse<List<ViewDocument>>(400, "No Documents Found", null);
                if (pastWeek != 0)
                {
                    userdocs = userdocs.Where(x => x.OrderDate >= DateTime.UtcNow.AddDays(-pastWeek)).ToList();
                }
                return new ServiceResponse<List<ViewDocument>>(200, "All User Documents", userdocs);
            }
            else
            {
                var documents = await _context.UserDocuments.Include(x => x.Document)
                                                           .Where(x => x.UserId == currentuser &&
                                                                       x.IsRecent == true)
                                                           .ToListAsync();
                var userdocs = documents.Select(x => new ViewDocument
                {
                    Id = x.Id,
                    DocumentId = x.Document.Id,
                    DocumentName = x.Document.DocumentName,
                    AlternateName = x.AlternateName,
                    Description = x.Document.Description,
                    DocumentPath = x.Document.DocumentPath,
                    OrderDate = x.OrderDate
                }).OrderByDescending(x => x.OrderDate).ToList();
                return new ServiceResponse<List<ViewDocument>>(200, "All User Documents", userdocs);
            }
        }

        public async Task<ServiceResponse<EnvelopeDetailsResponse>> UserSentDocumentById(string envId)
        {
            var currentUser = _userService.UserID;
            var mailedDocs = await _context.EnvolopeDetails.Include(x => x.Document)
                                                          .Include(x => x.User)
                                                          .Include(x => x.DocumentSignerDetails)
                                                          .Where(x => x.SentBy == currentUser &&
                                                          x.EnvelopeId == envId)
                                                          .Select(x => new EnvelopeDetailsResponse
                                                          {
                                                              EnvelopeId = x.EnvelopeId,
                                                              DocumentName = x.Document.DocumentName,
                                                              SentDocument = x.Document.DocumentPath,
                                                              Quantity = 1,
                                                              SentById = x.SentBy,
                                                              SentByName = x.User.FullName,
                                                              DocumentStatus = x.MailStatus,
                                                              SentTime = x.SentTime,
                                                              SignedDocumentURL = x.SignedDocumentURL,
                                                              Emails = x.DocumentSignerDetails.Select(x => new SignedRecepients
                                                              {
                                                                  EmailAddress = x.SignerEmail,
                                                                  SignerName = x.SignerName,
                                                                  SignatureStatus = x.SignatureStatus,
                                                                  SignatureTime = x.SignatureTime
                                                              }).ToList(),
                                                          })
                                                          .FirstOrDefaultAsync();
            if (mailedDocs == null)
            {
                return new ServiceResponse<EnvelopeDetailsResponse>(400, "Invalid Document Id / No Document Found", null);
            }
            return new ServiceResponse<EnvelopeDetailsResponse>(200, "Sent Document Details by EnvelopeId", mailedDocs);
        }

        public async Task<ServiceResponse<List<EnvelopeDetailsResponse>>> UserSentDocuments()
        {
            var currentUser = _userService.UserID;
            var mailedDocs = await _context.EnvolopeDetails.Include(x => x.Document)
                                                          .Include(x => x.User)
                                                          .Include(x => x.UserDocuments)
                                                          .Where(x => x.SentBy == currentUser)
                                                          .Select(x => new EnvelopeDetailsResponse
                                                          {
                                                              EnvelopeId = x.EnvelopeId,
                                                              DocumentId = x.DocumentId,
                                                              DocumentName = x.Document.DocumentName,
                                                              UserDocumentId = (long)x.UserDocumentId,
                                                              AlternateName = x.UserDocuments.AlternateName,
                                                              SentDocument = x.Document.DocumentPath,
                                                              Quantity = 1,
                                                              SentById = x.SentBy,
                                                              SentByName = x.User.FullName,
                                                              DocumentStatus = x.MailStatus,
                                                              SentTime = x.SentTime,
                                                              Emails = x.DocumentSignerDetails.Select(x => new SignedRecepients
                                                              {
                                                                  EmailAddress = x.SignerEmail,
                                                                  SignerName = x.SignerName,
                                                                  SignatureStatus = x.SignatureStatus,
                                                                  SignatureTime = x.SignatureTime
                                                              }).ToList(),
                                                              SignedDocumentURL = x.SignedDocumentURL
                                                          })
                                                          .OrderByDescending(x => x.UserDocumentId)
                                                          .ToListAsync();
            foreach (var mailedDoc in mailedDocs)
            {
                string email = "";
                string signerName = "";
                string status = "";
                string time = "";
                foreach (var abc in mailedDoc.Emails)
                {
                    email = email + abc.EmailAddress + ",";
                    signerName = signerName + abc.SignerName + ",";
                    status = status + abc.SignatureStatus + ",";
                    time = time + abc.SignatureTime + ",";
                }
                mailedDoc.Email = email;
                mailedDoc.SignerName = signerName;
                mailedDoc.SignTime = time;
                mailedDoc.SignerStatus = status;
                if (mailedDoc.Emails.Where(x => x.SignatureStatus == "pending").Any())
                {
                    mailedDoc.DocumentStatus = "pending";
                }
                else
                {
                    mailedDoc.DocumentStatus = "Signed";
                }
            }
            if (mailedDocs == null)
            {
                return new ServiceResponse<List<EnvelopeDetailsResponse>>(400, "No Documents Were Sent by User", null);
            }
            return new ServiceResponse<List<EnvelopeDetailsResponse>>(200, "All Sent Documents With Signature Status", mailedDocs);
        }

        public async Task<ServiceResponse<List<ViewCart>>> ViewCart()
        {
            var response = new List<ViewCart>();
            var cartItems = await _context.UserCart.Include(x => x.Document)
                .Where(x => x.UserId == _userService.UserID)
                .ToListAsync();
            foreach (var item in cartItems)
            {
                if (item.Quantity == 1)
                {
                    var newItem = new ViewCart()
                    {
                        UserViewCartId = item.Id,
                        UserId = item.UserId,
                        DocumentId = item.DocumentId,
                        DocumentName = item.Document.DocumentName,
                        Quantity = item.Quantity,
                        DocumentPhotoPath = item.Document.DocumentPath,
                        Price = item.Document.Price,
                        GasFee = item.Document.GasFee
                    };
                    response.Add(newItem);
                }
                else if (item.Quantity == 2)
                {
                    var newItem = new ViewCart()
                    {
                        UserViewCartId = item.Id,
                        UserId = item.UserId,
                        DocumentId = item.DocumentId,
                        DocumentName = item.Document.AlternateName,
                        Quantity = item.Quantity,
                        DocumentPhotoPath = item.Document.DocumentPath,
                        Price = 0,
                        GasFee = item.Document.GasFee
                    };
                    if(newItem.DocumentName == null)
                        newItem.DocumentName = item.Document.DocumentName;
                    response.Add(newItem);
                }
            }
            if (response.Count == 0)
            {
                return new ServiceResponse<List<ViewCart>>(200, "No Items in the Cart", null);
            }
            return new ServiceResponse<List<ViewCart>>(200, "Cart Items", response);
        }



        public async Task<ServiceResponse<ViewDocument>> ViewDocumentById(long Id)
        {
            var userId = _userService.UserID;
            List<SignedRecepients> result = new();
            var Doc = await _context.UserDocuments.Include(x => x.Document)
                                                 .Where(x => x.UserId == userId && x.Id == Id)
                                                .Select(x => new ViewDocument
                                                {
                                                    Id = x.Id,
                                                    DocumentId = x.Document.Id,
                                                    DocumentName = x.Document.DocumentName,
                                                    Description = x.Document.Description,
                                                    AlternateName = x.AlternateName,
                                                    DocumentPath = x.Document.DocumentPath,
                                                    CategoryId = x.Document.Category_Id,
                                                    OrderDate = x.OrderDate,
                                                    IsEdited = x.IsEdited,
                                                    NewDocument = x.EditedDocument
                                                }).FirstOrDefaultAsync();
            if (Doc == null)
                return new ServiceResponse<ViewDocument>(400, "Document not purchased", null);
            if (Doc.IsEdited == true)
            {
                Doc.DocumentPath = Doc.NewDocument;
            }
            var signaturestatus = await _context.EnvolopeDetails.Where(x => x.UserDocumentId == Doc.Id && x.SentBy == userId).ToListAsync();
            var signaturestatusId = signaturestatus.Select(x => x.Id).ToList();
            if (signaturestatus.Count != 0)
            {
                result = await _context.DocumentSignerDetails.Include(x => x.EnvolopeDetails)
                                                             .Where(x => signaturestatusId.Contains(x.EnvelopeId))
                                                             .Select(x => new SignedRecepients
                                                             {
                                                                 EmailAddress = x.SignerEmail,
                                                                 SignerName = x.SignerName,
                                                                 SignatureTime = x.SignatureTime,
                                                                 SignatureStatus = x.SignatureStatus
                                                             })
                                                             .ToListAsync();
                var status = result.Where(x => x.SignatureStatus == "pending").ToList();
                if (status.Count != 0)
                {
                    Doc.SignatureStatus = "pending";
                }
                else
                {
                    Doc.SignatureStatus = "Signed";
                }
                if (signaturestatus.Count >= 1)
                {
                    var URL = signaturestatus.OrderByDescending(x => x.SentTime).FirstOrDefault();
                    if (URL.SignedDocumentURL != null)
                        Doc.SignedDocumentUrl = URL.SignedDocumentURL;
                }
            }
            else if (signaturestatus.Count == 0)
            {
                result = null;
            }
            Doc.SignedRecepients = result;

            var hashdetails = await _context.HashDetails
                                           .Where(x => x.UserDocumentId == Id && x.UserId == _userService.UserID)
                                           .Select(x => new HashModel
                                           {
                                               BlockchainId = x.BlockchainId,
                                               Hash = x.Hash,
                                               IPFS = x.IPFS,
                                               BlockchainTrx = x.BlockchainTrx,
                                               Status = x.Status
                                           })
                                           .FirstOrDefaultAsync();
            if (hashdetails != null)
            {
                Doc.HashDetails = hashdetails;
            }
            else
            {
                Doc.HashDetails = null;
            }

            return new ServiceResponse<ViewDocument>(200, "Success", Doc);
        }

        public async Task<ServiceResponse<List<ViewNewDocumentRequest>>> ViewMyDocumentRequests()
        {
            var CurrentUser = _userService.UserID;
            var MyRequests = await _context.DocumentRequest.Include(x => x.RequestStatusMaster)
                                                          .Include(x => x.Category)
                                                          .Where(x => x.UserId == CurrentUser)
                                                          .Select(x => new ViewNewDocumentRequest
                                                          {
                                                              Id = x.Id,
                                                              Description = x.DocumentDescription,
                                                              Category = x.Category.Name,
                                                              CreatedDate = x.CreatedDate,
                                                              Status = x.RequestStatusMaster.Status,
                                                              LastStatusUpdated = x.UpdatedDate
                                                          })
                                                          .ToListAsync();
            if (MyRequests.Count == 0)
                return new ServiceResponse<List<ViewNewDocumentRequest>>(StatusCodes.Status404NotFound, "no Requests found", null);
            return new ServiceResponse<List<ViewNewDocumentRequest>>(StatusCodes.Status200OK, "all user Requests", MyRequests);
        }

        public async Task<ServiceResponse<ProfileViewModel>> ViewProfile()
        {
            var currentUser = _userService.UserID;
            var userProfile = await _context.User.Where(x => x.Id == currentUser)
                                                .Select(x => new ProfileViewModel
                                                {
                                                    UserName = x.FullName,
                                                    Email = x.EmailId,
                                                    PhoneNumber = x.ContactNumber,
                                                    ProfilePhoto = x.ProfilePicture,
                                                    Country = _context.Country.Where(c => c.Id == x.CountryId).Select(c => c.Name).FirstOrDefault()
                                                })
                                                .FirstOrDefaultAsync();

            return new ServiceResponse<ProfileViewModel>(200, "User Profile", userProfile);
        }


        public async Task<ApiResponse> SendLegalContactMail(LegalContactModel model)
        {
            var result = await _emailService.LegalContactMail(model);
            return result;
        }


        public async Task<ApiResponse> ContactUsEmail(ContactUsModel model)
        {
            var result = await _emailService.ContactUsEmail(model);
            return result;
        }

        public async Task<List<CountryModel>> GetCountry()
        {
            var query = await _context.Country.Select(s => new CountryModel
            {
                Id = s.Id,
                Country = s.Name,
                CountryCode = s.CountryCode
            }).ToListAsync();
            return query;
        }

        public async Task<ServiceResponse<ResendDocumentModel>> ResendDocument(int DocumentId)
        {
            var doc = await _context.Document.Where(x => x.Id == DocumentId)
                                             .Select(x => new ResendDocumentModel
                                             {
                                                 DocumentId = x.Id,
                                                 DocumentName = x.DocumentName,
                                                 GasFee = x.GasFee
                                             })
                                             .FirstOrDefaultAsync();
            if (doc == null)
                return new ServiceResponse<ResendDocumentModel>(StatusCodes.Status400BadRequest,
                                                                "invalid document id", null);
            return new ServiceResponse<ResendDocumentModel>(StatusCodes.Status200OK, "please confirm to proceed", doc);
        }

        public async Task<ApiResponse> ConfirmResend(ConfirmResentModel model)
        {
            var checkitem = await _context.UserCart.Where(x => x.UserId == _userService.UserID &&
                                                  x.DocumentId == model.DocumentId).FirstOrDefaultAsync();
            var userDoc = await _context.UserDocuments.Where(x => x.UserId == _userService.UserID &&
                                                                 x.DocumentId == model.DocumentId).FirstOrDefaultAsync();
            if (checkitem != null)
            {
                //checkitem.Quantity += quantity;
                //context.Update(checkitem);
                //await context.SaveChangesAsync();
                return new ApiResponse("Item already in cart", 400);
            }
            UserCart newItem = new()
            {
                UserId = _userService.UserID,
                DocumentId = model.DocumentId,
                CreatedDate = System.DateTime.UtcNow
            };
            if (userDoc != null)
            {
                newItem.Quantity = 2;
            }
            else
            {
                newItem.Quantity = 1;
            }
            var doc = await _context.Document.Where(x => x.Id == model.DocumentId).FirstOrDefaultAsync();
            doc.AlternateName = model.AlternateName;
            doc.UploadDate = DateTime.UtcNow;

            _context.Update(doc);
            await _context.AddAsync(newItem);
            await _context.SaveChangesAsync();
            return new ApiResponse("Added successfully to the Cart", 200);
        }
    }
}
