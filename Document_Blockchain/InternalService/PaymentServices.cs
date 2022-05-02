using Document_Blockchain.Entities;
using Document_Blockchain.IInternalServices;
using Document_Blockchain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Document_Blockchain.InternalService
{
    public class PaymentServices : IPaymentServices
    {
        private readonly BlockChainDbContext _context;
        private readonly IUserServices _userServices;
        private readonly IConfiguration _configuration;

        public PaymentServices(BlockChainDbContext context,
                               IUserServices userServices,
                               IConfiguration configuration)
        {
            this._context = context;
            this._userServices = userServices;
            this._configuration = configuration;
        }

        private async Task<ApiResponse> MakePaymentForOrder([FromBody] PaymentGatewayModel paymentRequest)
        {

            var CheckOrderId = await _context.PaymentLedger.Where(x => x.Id == paymentRequest.OrderId && x.UserId == _userServices.UserID).FirstOrDefaultAsync();

            if (CheckOrderId == null)
            {
                try
                {
                    var stripeCharge = CreateCharge(paymentRequest);

                    var save = SavePaymentDetails(stripeCharge, paymentRequest.OrderId, paymentRequest.Amount);


                    if (save.Result.TransactionStatus == "succeeded")
                    {
                        return new ApiResponse("succeeded", 200);
                    }
                    if (save.Result.TransactionStatus == "pending")
                    {
                        return new ApiResponse("pending", 201);
                    }
                    if (save.Result.TransactionStatus == "failed")
                    {
                        return new ApiResponse("failed", 400);
                    }
                }
                catch (Exception ex)
                {
                    return new ApiResponse(ex.Message, 400);
                }
            }
            return new ApiResponse("Already Payment is the Done For this Order  ", 400);

        }

        private Charge CreateCharge(PaymentGatewayModel paymentRequest)
        {
            var getStripeKey = _configuration.GetValue<string>("PaymentConfiguration:SecretKey");
            StripeConfiguration.ApiKey = getStripeKey;

            var myCharge = new ChargeCreateOptions();
            myCharge.Source = paymentRequest.TokenId;
            myCharge.Amount = ((long?)(paymentRequest.Amount * 100));
            myCharge.Currency = "USD";

            myCharge.Metadata = new Dictionary<string, string>
            {
                ["OurRef"] = "DocumentBlockChains-User:" + paymentRequest.OrderId + "/Id:" + Guid.NewGuid().ToString()
            };
            var chargeService = new ChargeService();
            Charge stripeCharge = chargeService.Create(myCharge);

            return stripeCharge;
        }

        private async Task<PaymentLedger> SavePaymentDetails(Charge stripeCharge, long orderId, decimal Amount)
        {
            PaymentLedger paymentLedger = new()
            {
                CreatedDate = DateTime.UtcNow,
                UserId = _userServices.UserID,
                TransactionStatus = stripeCharge.Status.ToString(),
                LastUpdateDate = DateTime.UtcNow,
                Amount = Amount,
                OrderId = orderId
            };

            await _context.PaymentLedger.AddAsync(paymentLedger);
            await _context.SaveChangesAsync();

            return paymentLedger;
        }


        public async Task<ApiResponse> PlaceOrder(PlaceOrderModel order)
        {

            UserOrders userOrders = new()
            {
                CreatedDate = DateTime.UtcNow,
                OrderAmount = order.OrderAmount,
                OrderSatus = "Payment Inititated",
                UserId = _userServices.UserID
            };
            await _context.UserOrders.AddAsync(userOrders);
            await _context.SaveChangesAsync();

            PaymentGatewayModel paymentRequest = new PaymentGatewayModel();
            paymentRequest.Amount = order.OrderAmount;
            paymentRequest.OrderId = userOrders.Id;
            paymentRequest.TokenId = order.TokenId;
            var placeOrder = MakePaymentForOrder(paymentRequest);

            if (placeOrder.Result.Message == "succeeded")
            {
                await UpdateData(userOrders.Id, order.CartIds, placeOrder.Result.Message);
                return new ApiResponse(placeOrder.Result.Message, 201);
            }
            if (placeOrder.Result.Message == "pending")
            {
                await UpdateData(userOrders.Id, order.CartIds, placeOrder.Result.Message);
                return new ApiResponse(placeOrder.Result.Message, 201);
            }
            if (placeOrder.Result.Message == "failed")
            {
                await UpdateData(userOrders.Id, order.CartIds, placeOrder.Result.Message);
                return new ApiResponse(placeOrder.Result.Message, 400);
            }
            else
            {
                await UpdateData(userOrders.Id, order.CartIds, placeOrder.Result.Message);
                return new ApiResponse(placeOrder.Result.Message, 400);
            }
        }


        private async Task<bool> UpdateData(long id, List<MultipleCart> cartIds, string message)
        {
            var getOrder = _context.UserOrders.Where(x => x.Id == id).FirstOrDefault();

            getOrder.OrderSatus = message;
            _context.UserOrders.Update(getOrder);

            if (message == "succeeded")
            {
                if (cartIds.Count > 0)
                {
                    var userDocs = await _context.UserDocuments.Where(x => x.UserId == _userServices.UserID &&
                                                                           x.IsRecent == true)
                                                               .ToListAsync();
                    if (userDocs.Count != 0)
                    {
                        foreach (var doc in userDocs)
                        {
                            doc.IsRecent = false;
                        }
                        _context.UpdateRange(userDocs);
                    }
                    foreach (var removeFromCart in cartIds)
                    {
                        var abc = await _context.UserCart.Where(x => x.Id == removeFromCart.Id)
                                                         .Select(x => new { cartid = x.Id, documentId = x.DocumentId })
                                                         .ToListAsync();
                        var docId = abc.Where(x => x.cartid == removeFromCart.Id).Select(x => x.documentId).FirstOrDefault();
                        UserDocuments document = new()
                        {
                            UserId = _userServices.UserID,
                            DocumentId = docId,
                            Quantity = 1,
                            RemainingQuantity = 1,
                            IsRecent = true,
                            OrderId = id,
                            OrderDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow
                        };

                        var doc = await _context.Document.Where(x => x.Id == docId).FirstOrDefaultAsync();
                        document.AlternateName = doc.AlternateName;
                        doc.AlternateName = null;
                        _context.Update(doc);

                        var cartdetails = await _context.UserCart.Where(x => x.Id == removeFromCart.Id).FirstOrDefaultAsync();
                        if (cartdetails.Quantity == 1)
                        {
                            TransactionLog newLog = new()
                            {
                                Amount = await _context.UserCart.Include(x => x.Document).Where(x => x.Id == cartdetails.Id).Select(x => (x.Document.Price + x.Document.GasFee)).FirstOrDefaultAsync(),
                                CreatedDate = DateTime.UtcNow,
                                UserId = _userServices.UserID,
                                OrderId = id,
                                DocumentId = document.DocumentId,
                                OrderStatus = message,
                                Quantity = 0
                            };
                            await _context.AddAsync(newLog);
                        }
                        else if (cartdetails.Quantity == 2)
                        {
                            TransactionLog newLog = new()
                            {
                                Amount = await _context.UserCart.Include(x => x.Document).Where(x => x.Id == cartdetails.Id).Select(x => x.Document.GasFee).FirstOrDefaultAsync(),
                                CreatedDate = DateTime.UtcNow,
                                UserId = _userServices.UserID,
                                OrderId = id,
                                DocumentId = document.DocumentId,
                                OrderStatus = message,
                                Quantity = 0
                            };
                            await _context.AddAsync(newLog);
                        }


                        var getRemovableCarts = await _context.UserCart.Where(x => x.Id == removeFromCart.Id).FirstOrDefaultAsync();
                        await _context.UserDocuments.AddAsync(document);
                        _context.UserCart.Remove(getRemovableCarts);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                if (cartIds.Count > 0)
                {
                    foreach (var cart in cartIds)
                    {
                        var getCart = await _context.UserCart.Where(x => x.UserId == _userServices.UserID && x.Id == cart.Id).FirstOrDefaultAsync();
                        getCart.OrderStatus = message;
                        getCart.UserOrderId = id;
                        _context.UserCart.Update(getCart);
                    }
                }
            }
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<SubscriptionTokenModel>> GetListSubsciptiontoken()
        {
            var getsubscription = await _context.SubscriptionToken.Where(x => x.IsActive == true).Select(x => new SubscriptionTokenModel
            {

                Id = x.Id,
                PublishKey = x.PublishKey,
                SecretKey = x.SecretKey


            }).ToListAsync();

            return getsubscription;
        }
    }
}
