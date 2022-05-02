using Newtonsoft.Json;
using System;

namespace Document_Blockchain.Models
{
    public class UserModel
    {
    }

    public class LoginOtpResponse
    {
        [JsonProperty("emailId")]
        public string EmailId { get; set; }

        [JsonProperty("loginResponse")]
        public LoginResponse LoginResponse { get; set; }
    }

    public class ResendOTPModel
    {
        [JsonProperty("emailId")]
        public string EmailId { get; set; }
    }


    public class OTPModel
    {
        [JsonProperty("emailId")]
        public string EmailId { get; set; }

        [JsonProperty("otp")]
        public string OTP { get; set; }
    }


    public class ContactUsModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public long PhoneNumber { get; set; }

        [JsonProperty("pleaseProvideConcern")]
        public string PleaseProvideConcern { get; set; }
    }


    public class NewRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public long PhoneNumber { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class LegalContactModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public long PhoneNumber { get; set; }

        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
    public class ProfileViewModel
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public long PhoneNumber { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("profilePhoto")]
        public string ProfilePhoto { get; set; }
    }

    public class ProfileUpdateModel
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("phoneNumber")]
        public long PhoneNumber { get; set; }

        [JsonProperty("country")]
        public long Country { get; set; }

        [JsonProperty("profilePhoto")]
        public string ProfilePhoto { get; set; }

        [JsonProperty("extention")]
        public string Extention { get; set; }
    }

    public class PaymentLedgerResponse
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        [JsonProperty("transactionStatus")]
        public string TransactionStatus { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }

    public class CountryModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
    }

}
