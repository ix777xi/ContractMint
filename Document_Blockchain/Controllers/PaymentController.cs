using Document_Blockchain.Constants;
using Document_Blockchain.IInternalServices;
using Document_Blockchain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Document_Blockchain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [Authorize(Roles = "User")]
        [HttpPost("[action]")]
        public async Task<ApiResponse> PlaceOrder(PlaceOrderModel order)
        {
            if (!ModelState.IsValid)
                return new ApiResponse(ApiResponseConstants.invalidInput, 400);
            var result = await _unitOfWork.PaymentServices.PlaceOrder(order);
            return result;
        }



        [HttpGet("[action]")]
        public async Task<List<SubscriptionTokenModel>> GetListSubsciptiontoken()
        {
            var getsubscription = await _unitOfWork.PaymentServices.GetListSubsciptiontoken();

            return getsubscription;
        }

    }
}

