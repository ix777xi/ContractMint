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
    public class AdminServices : IAdminServices
    {
        private readonly IStorage _storage;
        private readonly IUserServices _userService;
        private readonly IEmailServices _emailService;
        private readonly BlockChainDbContext _context;

        public AdminServices(IStorage storage,
                             IUserServices userService,
                             IEmailServices emailService,
                             BlockChainDbContext context)
        {
            _storage = storage;
            _userService = userService;
            _emailService = emailService;
            _context = context;
        }

        public async Task<ApiResponse> AddCategory(AddCatModel model)
        {
            var checkCat = await _context.Category.Where(x => x.Name == model.CategoryName).FirstOrDefaultAsync();
            if (checkCat != null)
                return new ApiResponse("category already exists", 400);
            Category newcat = new()
            {
                Name = model.CategoryName,
                IsPrimary = model.IsPrimary
            };
            if (model.Photo != null)
            {
                var photopath = await _storage.UploadFile($"substorage/Documentstorage", model.Photo, ".png");
                newcat.PhotoPath = photopath.Data;
            }
            await _context.AddAsync(newcat);
            await _context.SaveChangesAsync();
            return new ApiResponse("added successfully", 200);
        }

        public async Task<ApiResponse> AddNewDocument(NewDocument model)
        {
            var checkDoc = await _context.Document.Where(x => x.DocumentName == model.DocumentName).FirstOrDefaultAsync();
            if (checkDoc != null)
                return new ApiResponse("Document Already exists", 400);

            Document newDoc = new()
            {
                DocumentName = model.DocumentName,
                UploadDate = DateTime.UtcNow,
                Description = model.Description,
                Price = model.Price,
                GasFee = model.GasFee,
                Category_Id = model.CategoryId,
                IsDeleted = false,
                IsPrimary = model.IsPrimary
            };

            var docPath = await _storage.UploadFile($"substorage/Documentstorage", model.Document, model.DocumentExtention);
            if (docPath.StatusCode != 200)
                return new ApiResponse(docPath.Message, 400);

            newDoc.DocumentPath = docPath.Data;
            await _context.AddAsync(newDoc);
            await _context.SaveChangesAsync();

            return new ApiResponse("added Successfully", 200);
        }

        public async Task<ServiceResponse<List<PaymentLedgerResponse>>> AllPaymentDetails()
        {

            var currentUser = _userService.UserID;
            var userRole = await _context.User.Include(x => x.UserRole)
                                               .ThenInclude(x => x.Role)
                                               .Where(x => x.Id == currentUser)
                                               .Select(x => x.UserRole.Select(x => x.Role.Name).FirstOrDefault())
                                               .FirstOrDefaultAsync();
            if (userRole != "Admin")
                return new ServiceResponse<List<PaymentLedgerResponse>>(400, "user not authorized", null);
            var allPaymentDetails = await _context.PaymentLedger.Include(x => x.User).Select(x => new PaymentLedgerResponse
            {
                UserName = x.User.FullName,
                OrderId = x.OrderId,
                TransactionStatus = x.TransactionStatus,
                Amount = x.Amount,
                CreatedDate = x.CreatedDate
            }).ToListAsync();
            return new ServiceResponse<List<PaymentLedgerResponse>>(200, "all payment Details", allPaymentDetails);
        }

        public async Task<ServiceResponse<List<ProfileViewModel>>> AllUsers()
        {
            var currentUser = _userService.UserID;
            var userRole = await _context.User.Include(x => x.UserRole)
                                               .ThenInclude(x => x.Role)
                                               .Where(x => x.Id == currentUser)
                                               .Select(x => x.UserRole.Select(x => x.Role.Name).FirstOrDefault())
                                               .FirstOrDefaultAsync();
            if (userRole != "Admin")
                return new ServiceResponse<List<ProfileViewModel>>(400, "user not authorized", null);

            var allUsers = await _context.User.Include(x => x.Country)
                                               .Where(x => x.Isadmin == false)
                                               .Select(x => new ProfileViewModel
                                               {
                                                   UserName = x.FullName,
                                                   Email = x.EmailId,
                                                   Country = x.Country.Name,
                                                   PhoneNumber = x.ContactNumber,
                                                   ProfilePhoto = x.ProfilePicture
                                               })
                                               .ToListAsync();

            return new ServiceResponse<List<ProfileViewModel>>(200, "all registered users", allUsers);
        }


        public async Task<ApiResponse> DeleteDocument(int Id)
        {
            var doc = await _context.Document.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (doc == null)
                return new ApiResponse("document not found", 400);

            doc.IsDeleted = true;
            _context.Update(doc);
            await _context.SaveChangesAsync();


            return new ApiResponse("deleted Successfully", 200);
        }

        public async Task<ApiResponse> EditDocumentRequestStatus(long id, long status)
        {
            var request = await _context.DocumentRequest.Where(x => x.Id == id).FirstOrDefaultAsync();
            request.Status = status;
            _context.Update(request);
            await _context.SaveChangesAsync();
            var user = await _context.User.Where(x => x.Id == request.UserId).FirstOrDefaultAsync();

            await _emailService.StatusChangeMail(user.EmailId, await _context.RequestStatusMaster.Where(x => x.Id == request.Status).Select(x => x.Status).FirstOrDefaultAsync());

            return new ApiResponse("Status Updated", StatusCodes.Status200OK);
        }

        public async Task<ServiceResponse<List<StatusDropDownModel>>> RequestStatusDropDown()
        {
            var data = await _context.RequestStatusMaster.Select(x => new StatusDropDownModel
            {
                Id = x.Id,
                Status = x.Status
            }).ToListAsync();
            return new ServiceResponse<List<StatusDropDownModel>>(StatusCodes.Status200OK, "status DropDown", data);
        }

        public async Task<ApiResponse> UpdateCategory(EditCatModel model)
        {
            var cat = await _context.Category.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
            if (cat == null)
                return new ApiResponse("category not found", StatusCodes.Status400BadRequest);
            cat.Name = model.Categoryname;
            cat.IsPrimary = model.IsPrimary;
            if (model.Photo != null || model.Photo != "")
            {
                var fileUpload = await _storage.UploadFile($"substorage/Documentstorage", model.Photo, model.Extenction);
                if (fileUpload.StatusCode == 200)
                {
                    cat.PhotoPath = fileUpload.Data;
                }
            }
            _context.Update(cat);
            await _context.SaveChangesAsync();
            return new ApiResponse("Updated Successfully", StatusCodes.Status200OK);
        }


        public async Task<ApiResponse> DeleteCategory(long categoryId)
        {
            var cat = await _context.Category.Where(x => x.Id == categoryId).FirstOrDefaultAsync();
            if (cat == null)
                return new ApiResponse("invalid category id", StatusCodes.Status400BadRequest);
            cat.IsDeleted = true;
            _context.Update(cat);
            await _context.SaveChangesAsync();
            return new ApiResponse("Category Deleted Successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse> UpdateDocument(UpdateDocument model)
        {
            var doc = await _context.Document.Where(x => x.Id == model.DocumentId).FirstOrDefaultAsync();
            if (doc == null)
                return new ApiResponse("document doesnot exists", 400);

            doc.DocumentName = model.DocumentName;
            doc.UploadDate = DateTime.UtcNow;
            doc.Description = model.Description;
            doc.Price = model.Price;
            doc.GasFee = model.GasFee;
            doc.Category_Id = model.CategoryId;
            doc.IsDeleted = false;
            doc.IsPrimary = model.IsPrimary;
            if (model.Document != null)
            {
                //var removedoc = await storage.DeleteFile(doc.DocumentPath);

                var docPath = await _storage.UploadFile($"substorage/Documentstorage", model.Document, model.DocumentExtention);

                if (docPath.StatusCode != 200)
                    return new ApiResponse(docPath.Message, 400);
                doc.DocumentPath = docPath.Data;
            }

            _context.Update(doc);
            await _context.SaveChangesAsync();
            return new ApiResponse("updated successfully", 200);
        }

        public async Task<ServiceResponse<List<Categories>>> ViewAllCategories(ViewAllCategoriesModel model)
        {
            List<Categories> categories = new List<Categories>();

            if (model.IsPrimary == false)
            {
                categories = await _context.Category.Where(x => x.IsDeleted != true).Select(x => new Categories
                {
                    CategoryId = x.Id,
                    CategoryName = x.Name,
                    PhotoPath = x.PhotoPath,
                    IsPrimary = x.IsPrimary
                }).ToListAsync();
            }
            else if (model.IsPrimary == true)
            {
                categories = await _context.Category.Where(x => x.IsDeleted != true && x.IsPrimary == true).Select(x => new Categories
                {
                    CategoryId = x.Id,
                    CategoryName = x.Name,
                    PhotoPath = x.PhotoPath
                }).ToListAsync();
            }
            return new ServiceResponse<List<Categories>>(200, "all primary categories", categories);

        }

        public async Task<ServiceResponse<List<ViewAdminDocument>>> ViewAllDocuments()
        {
            var allDocs = await _context.Document.Where(x => x.IsDeleted == false)
                .Select(x => new ViewAdminDocument
                {
                    Id = x.Id,
                    DocumentName = x.DocumentName,
                    Description = x.Description,
                    Price = x.Price,
                    GasFee = x.GasFee,
                    DocumentPath = x.DocumentPath,
                    CategoryId = x.Category.Id,
                    Category = x.Category.Name,
                    IsPrimary = x.IsPrimary,
                    CreatedDate = x.UploadDate
                }).OrderByDescending(x => x.CreatedDate).ToListAsync();

            return new ServiceResponse<List<ViewAdminDocument>>(200, "all documents", allDocs);
        }


        public async Task<ServiceResponse<List<ViewDocument>>> ViewAllDocumentsByCat(ViewDocumentsByCategoryModel model)
        {
            List<ViewDocument> allDocs = new List<ViewDocument>();

            if (model.IsPrimary == false)
            {
                allDocs = await _context.Document.Include(x => x.Category)
                                                    .Where(x => x.Category_Id == model.CategoryId && x.IsDeleted == false && x.Category.IsDeleted != true)
                                                    .Select(x => new ViewDocument
                                                    {
                                                        Id = x.Id,
                                                        DocumentName = x.DocumentName,
                                                        Description = x.Description,
                                                        Price = x.Price,
                                                        GasFee = x.GasFee,
                                                        DocumentPath = x.DocumentPath,
                                                        CategoryId = x.Category_Id,
                                                        IsPrimary = x.IsPrimary
                                                    }).ToListAsync();
            }
            else if (model.IsPrimary == true)
            {
                allDocs = await _context.Document.Include(x => x.Category)
                                                    .Where(x => x.Category_Id == model.CategoryId &&
                                                                x.IsDeleted == false &&
                                                                x.Category.IsDeleted != true &&
                                                                x.IsPrimary == true)
                                                    .Select(x => new ViewDocument
                                                    {
                                                        Id = x.Id,
                                                        DocumentName = x.DocumentName,
                                                        Description = x.Description,
                                                        Price = x.Price,
                                                        GasFee = x.GasFee,
                                                        DocumentPath = x.DocumentPath,
                                                        CategoryId = x.Category_Id
                                                    }).ToListAsync();
            }
            return new ServiceResponse<List<ViewDocument>>(200, "all documents", allDocs);
        }


        public async Task<ServiceResponse<List<ViewDocumentRequest>>> ViewDocumentRequests()
        {
            var CurrentUser = _userService.UserID;
            var MyRequests = await _context.DocumentRequest.Include(x => x.RequestStatusMaster)
                                                          .Include(x => x.User)
                                                          .Include(x => x.Category)
                                                          .Select(x => new ViewDocumentRequest
                                                          {
                                                              Id = x.Id,
                                                              Description = x.DocumentDescription,
                                                              CreatedDate = x.CreatedDate,
                                                              Category = x.Category.Name,
                                                              Status = x.RequestStatusMaster.Status,
                                                              LastStatusUpdated = x.UpdatedDate,
                                                              RequestedBy = x.User.FullName,
                                                              RequestedByEmail = x.User.EmailId
                                                          })
                                                          .ToListAsync();
            if (MyRequests.Count == 0)
                return new ServiceResponse<List<ViewDocumentRequest>>(StatusCodes.Status404NotFound, "no Requests found", null);
            return new ServiceResponse<List<ViewDocumentRequest>>(StatusCodes.Status200OK, "all user Requests", MyRequests);
        }

        public async Task<ApiResponse> AddPrimaryCategory(int categoryId)
        {
            try
            {
                var primaryCategories = await _context.Category.Where(x => x.IsDeleted == false && x.IsPrimary == true).ToListAsync();
                if (primaryCategories.Count >= 4)
                    return new ApiResponse("only 4 primary categories can be added", StatusCodes.Status400BadRequest);
                var category = await _context.Category.Where(x => x.Id == categoryId &&
                                                                 x.IsDeleted == false &&
                                                                 x.IsPrimary == true).FirstOrDefaultAsync();
                if (category != null)
                    return new ApiResponse("this category is already a primary category", StatusCodes.Status208AlreadyReported);
                var newPrimaryCategory = await _context.Category.Where(x => x.Id == categoryId &&
                                                                           x.IsDeleted == false &&
                                                                           x.IsPrimary == false)
                                                               .FirstOrDefaultAsync();
                newPrimaryCategory.IsPrimary = true;
                _context.Update(newPrimaryCategory);
                await _context.SaveChangesAsync();
                return new ApiResponse("primary category added successfully", StatusCodes.Status200OK);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<ApiResponse> AddPrimaryDocument(int documentId)
        {
            var newPrimaryDocument = await _context.Document.Include(x => x.Category)
                                                           .Where(x => x.Id == documentId &&
                                                                       x.IsDeleted == false &&
                                                                       x.IsPrimary == false)
                                                           .FirstOrDefaultAsync();

            var primaryDocuments = await _context.Document.Where(x => x.IsDeleted == false &&
                                                                     x.IsPrimary == true &&
                                                                     x.Category_Id == newPrimaryDocument.Category_Id)
                                                         .ToListAsync();
            if (primaryDocuments.Count >= 4)
                return new ApiResponse("only 4 primary categories can be added", StatusCodes.Status400BadRequest);
            if (newPrimaryDocument.IsPrimary == true)
                return new ApiResponse("this category is already a primary category", StatusCodes.Status208AlreadyReported);

            newPrimaryDocument.IsPrimary = true;
            _context.Update(newPrimaryDocument);
            await _context.SaveChangesAsync();
            return new ApiResponse("primary category added successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse> RemovePrimaryCategory(int categoryId)
        {
            var category = await _context.Category.Where(x => x.Id == categoryId && x.IsPrimary == true).FirstOrDefaultAsync();
            if (category == null)
                return new ApiResponse("invalid category id", StatusCodes.Status400BadRequest);
            category.IsPrimary = false;
            _context.Update(category);
            await _context.SaveChangesAsync();
            return new ApiResponse("removed successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse> RemovePrimaryDocument(int documentId)
        {
            var Document = await _context.Document.Where(x => x.Id == documentId && x.IsPrimary == true).FirstOrDefaultAsync();
            if (Document == null)
                return new ApiResponse("invalid document id", StatusCodes.Status400BadRequest);
            Document.IsPrimary = false;
            _context.Update(Document);
            await _context.SaveChangesAsync();
            return new ApiResponse("removed successfully", StatusCodes.Status200OK);
        }
    }
}
