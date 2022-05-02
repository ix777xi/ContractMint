using Newtonsoft.Json;
using System.Collections.Generic;

namespace Document_Blockchain.Models
{
    public class PaymentGatewayModel
    {
        [JsonProperty("tokenId")]
        public string TokenId { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class PlaceOrderModel
    {
        [JsonProperty("tokenId")]
        public string TokenId { get; set; }

        [JsonProperty("orderAmount")]
        public decimal OrderAmount { get; set; }

        [JsonProperty("cartIds")]
        public List<MultipleCart> CartIds { get; set; }
    }

    public class PlaceRepeatOrderModel
    {
        [JsonProperty("tokenId")]
        public string TokenId { get; set; }

        [JsonProperty("documentId")]
        public long DocumentId { get; set; }
    }

    public class MultipleCart
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public class SubscriptionTokenModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("secretKey")]
        public string SecretKey { get; set; }

        [JsonProperty("publishKey")]
        public string PublishKey { get; set; }

    }
}
