using Document_Blockchain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Document_Blockchain.IInternalServices
{
    public interface IPaymentServices
    {
        Task<ApiResponse> PlaceOrder(PlaceOrderModel order);


        Task<List<SubscriptionTokenModel>> GetListSubsciptiontoken();


    }
}
