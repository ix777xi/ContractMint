using Document_Blockchain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Document_Blockchain.IInternalServices
{
    public interface IAdminServices
    {
        Task<ServiceResponse<List<Categories>>> ViewAllCategories(ViewAllCategoriesModel model);

        Task<ServiceResponse<List<ViewAdminDocument>>> ViewAllDocuments();

        Task<ServiceResponse<List<ViewDocument>>> ViewAllDocumentsByCat(ViewDocumentsByCategoryModel model);

        Task<ApiResponse> AddNewDocument(NewDocument model);

        Task<ApiResponse> UpdateDocument(UpdateDocument model);

        Task<ApiResponse> DeleteDocument(int Id);

        Task<ApiResponse> AddCategory(AddCatModel model);

        Task<ApiResponse> UpdateCategory(EditCatModel model);

        Task<ApiResponse> DeleteCategory(long categoryId);


        Task<ServiceResponse<List<ProfileViewModel>>> AllUsers();

        Task<ServiceResponse<List<PaymentLedgerResponse>>> AllPaymentDetails();

        Task<ServiceResponse<List<ViewDocumentRequest>>> ViewDocumentRequests();

        Task<ApiResponse> EditDocumentRequestStatus(long id, long status);

        Task<ServiceResponse<List<StatusDropDownModel>>> RequestStatusDropDown();

        Task<ApiResponse> AddPrimaryCategory(int categoryId);

        Task<ApiResponse> RemovePrimaryCategory(int categoryId);

        Task<ApiResponse> AddPrimaryDocument(int documentId);

        Task<ApiResponse> RemovePrimaryDocument(int documentId);



    }
}
