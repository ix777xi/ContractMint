using Newtonsoft.Json;
using System.Collections.Generic;
namespace Document_Blockchain.Models
{
    public class RegisterModel
    {
        [JsonProperty("emailId")]
        public string EmailId { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("internationalDailingCode")]
        public string InternationalDailingCode { get; set; }

        [JsonProperty("contactNumber")]
        public long ContactNumber { get; set; }

        [JsonProperty("isAdmin")]
        public bool IsAdmin { get; set; }

        [JsonProperty("roleId")]
        public int RoleId { get; set; }

        [JsonProperty("countryId")]
        public int CountryId { get; set; }
    }

    public class ChangePasswordModel
    {
        [JsonProperty("currentPassword")]
        public string CurrentPassword { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class LoginModel
    {
        [JsonProperty("emailId")]
        public string EmailId { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("expiresIn")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        [JsonProperty("tokenType")]
        public string TokenType { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }

    public class ResetPasswordModel
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
