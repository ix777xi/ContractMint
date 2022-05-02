using Document_Blockchain.Entities;
using Document_Blockchain.IInternalServices;
using Document_Blockchain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Document_Blockchain.InternalService
{

    public class AccountService : IAccountService
    {
        private readonly IEmailServices _emailService;
        private readonly BlockChainDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountService(IConfiguration configuration,
                              IEmailServices emailService,
                              BlockChainDbContext context)
        {
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
        }

        public async Task<ApiResponse> Register(RegisterModel model)
        {
            if (model.ContactNumber.ToString().Length > 14 || model.ContactNumber.ToString().Length < 10)
                return new ApiResponse("Please Enter Valid Phone Number", StatusCodes.Status400BadRequest);

            var checkUser = await _context.User.Where(x => x.EmailId == model.EmailId).FirstOrDefaultAsync();
            if (checkUser is not null)
                return new ApiResponse("The email or phonenumber already exists", 400);

            var userRole = await _context.Role.Where(x => x.Id == model.RoleId).FirstOrDefaultAsync();
            if (userRole == null)
                return new ApiResponse("Role not found", 403);
            var user = new User
            {
                EmailId = model.EmailId,
                Password = Encipher(model.Password),
                InternationalDailingCode = model.InternationalDailingCode,
                ContactNumber = model.ContactNumber,
                FullName = model.FullName,
                CreatedDate = DateTime.UtcNow,
                IsActive = false,
                Isadmin = model.IsAdmin,
                CountryId = model.CountryId
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            UserRole role = new()
            {
                RoleId = userRole.Id,
                UserId = user.Id
            };
            await _context.AddAsync(role);

            PasswordResetToken token = new()
            {
                Token = Guid.NewGuid().ToString(),
                Username = user.EmailId,
                ExpiryDate = DateTime.UtcNow.AddHours(3)
            };

            await _context.PasswordResetToken.AddAsync(token);
            await _context.SaveChangesAsync();

            await _emailService.VerifyRegistrationEmail($"{user.FullName}",
            user.EmailId, $"{_configuration.GetValue<string>("Domain:Angular")}/verify-email?email={user.EmailId}&token={token.Token}");

            return new ApiResponse("Registration Successfull", 200);
        }

        public async Task<ServiceResponse<LoginOtpResponse>> Login(LoginModel model)
        {
            string Responsemessege = "";
            var appUser = await _context.User.Where(x => x.EmailId == model.EmailId)
                                            .Include(x => x.UserRole)
                                            .ThenInclude(x => x.Role)
                                            .FirstOrDefaultAsync();

            if (appUser is not null && (Decipher(appUser.Password) == model.Password))
            {
                if (appUser.Is_EmailVerified != true)
                    return new ServiceResponse<LoginOtpResponse>(StatusCodes.Status401Unauthorized,
                                                        "Email not Verified, Please Verify your Email", null);

                var loginValid = await _context.UserAuthentications.Where(x => x.UserId == appUser.Id &&
                                                                        x.ExpireTime >= DateTime.UtcNow)
                                                                  .FirstOrDefaultAsync();

                if (loginValid == null && appUser.UserRole.Where(x => x.RoleId == 1).FirstOrDefault() == null)
                {
                    var r = new Random();
                    long otp = r.Next(100000, 199999);
                    var newOtp = otp.ToString();
                    UserAuthentication userAuthentication = new()
                    {
                        UserId = appUser.Id,
                        OTP = newOtp,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        ExpireTime = DateTime.UtcNow.AddMinutes(2)
                    };

                    await _context.AddAsync(userAuthentication);
                    await _context.SaveChangesAsync();

                    TwilioClient.Init(_configuration.GetValue<string>("Twilio:AccountSID"),
                    _configuration.GetValue<string>("Twilio:AuthToken"));

                    try
                    {
                        var messege = MessageResource.Create(
                            body: $"Your OTP for contractmint.io is {userAuthentication.OTP}",
                            from: _configuration.GetValue<string>("Twilio:TwilioPhoneNumber"),
                            to: $"{appUser.InternationalDailingCode}{appUser.ContactNumber}");

                    }
                    catch
                    {
                        var messege = "Failed to send OTP to Phone Number xxxxxxxx";
                        Responsemessege = messege;
                    }
                    await _emailService.ResendVerifyOTPMail(appUser.EmailId, userAuthentication.OTP);

                    var response = new LoginOtpResponse
                    {
                        EmailId = appUser.EmailId,
                        LoginResponse = null
                    };
                    var endNumb = appUser.ContactNumber.ToString()[^2..];
                    var email = string.Concat(appUser.EmailId.AsSpan(0, 2), "********.", appUser.EmailId.Split(".").Last());
                    if (Responsemessege.Length > 1)
                        return new ServiceResponse<LoginOtpResponse>(StatusCodes.Status200OK,
                                $"{Responsemessege}{endNumb}, but OTP was sent E-Mail {email}", response);

                    return new ServiceResponse<LoginOtpResponse>(StatusCodes.Status200OK,
                        $"A One Time Password (OTP) has been sent to your mobile number xxxxxxxx{endNumb} and E-Mail {email}", response);
                }
                else if (loginValid != null || appUser.UserRole.Where(x => x.RoleId == 1).FirstOrDefault() != null)
                {
                    if (loginValid != null && loginValid.IsActive == true && loginValid.ExpireTime >= DateTime.UtcNow)
                        return new ServiceResponse<LoginOtpResponse>(StatusCodes.Status400BadRequest,
                                                                    "Your One Time Password (OTP) has already been sent. Please Verify", null);

                    LoginOtpResponse result = new();
                    List<string> roles = appUser.UserRole.Select(x => x.Role.Name).ToList();

                    var refreshToken = GenerateRefreshToken();
                    appUser.RefreshToken.Add(refreshToken);
                    await _context.SaveChangesAsync();
                    var response = new LoginResponse
                    {
                        AccessToken = AccessToken(appUser, roles),
                        ExpiresIn = 3600,
                        RefreshToken = refreshToken.Token,
                        Roles = roles,
                        TokenType = "Bearer",
                        Username = $"{appUser.EmailId}"
                    };
                    result.LoginResponse = response;
                    result.EmailId = appUser.EmailId;
                    return new ServiceResponse<LoginOtpResponse>(StatusCodes.Status200OK,
                                            "Please Login With Access Token", result);

                }

                return new ServiceResponse<LoginOtpResponse>(StatusCodes.Status200OK,
                                            "Please enter a valid username & password", null);


            }
            else
            {
                return new ServiceResponse<LoginOtpResponse>(StatusCodes.Status404NotFound,
                    "please enter a valid user-name and password", null);
            }
        }


        public async Task<ServiceResponse<LoginResponse>> VerifyOTP(OTPModel model)
        {

            var appUser = await _context.User.Where(x => x.EmailId == model.EmailId).Include(x => x.UserRole).ThenInclude(x => x.Role).FirstOrDefaultAsync();
            if (appUser != null)
            {
                var otpdata = await _context.UserAuthentications.Where(x => x.UserId == appUser.Id &&
                                                                           x.ExpireTime >= DateTime.UtcNow &&
                                                                           x.IsActive == true)
                                                               .OrderByDescending(x => x.CreatedDate)
                                                               .FirstOrDefaultAsync();
                if (otpdata == null)
                {
                    return new ServiceResponse<LoginResponse>(StatusCodes.Status400BadRequest,
                        "OTP expired", null);
                }
                else if (otpdata.OTP == model.OTP)
                {
                    otpdata.IsActive = false;
                    otpdata.ExpireTime = DateTime.UtcNow.AddMinutes(5);
                    var userRole = await _context.UserRole.Where(x => x.UserId == appUser.Id)
                                                         .Select(x => x.Role.Name)
                                                         .FirstOrDefaultAsync();

                    List<string> roles = appUser.UserRole.Select(x => x.Role.Name).ToList();

                    var refreshToken = GenerateRefreshToken();
                    appUser.RefreshToken.Add(refreshToken);

                    _context.Update(otpdata);
                    await _context.SaveChangesAsync();
                    var response = new LoginResponse
                    {
                        AccessToken = AccessToken(appUser, roles),
                        ExpiresIn = 3600,
                        RefreshToken = refreshToken.Token,
                        Roles = roles,
                        TokenType = "Bearer",
                        Username = $"{appUser.EmailId}"
                    };
                    return new ServiceResponse<LoginResponse>(StatusCodes.Status200OK, "please login using a token", response);
                }
                else
                {
                    return new ServiceResponse<LoginResponse>(StatusCodes.Status400BadRequest, "OTP not valid", null);
                }
            }
            else
            {
                return new ServiceResponse<LoginResponse>(StatusCodes.Status404NotFound, "user not found", null);
            }
        }


        public async Task<ServiceResponse<LoginOtpResponse>> ResendVerifyOTP(ResendOTPModel model)
        {
            var appUser = await _context.User.Where(x => x.EmailId == model.EmailId)
                                            .Include(x => x.UserRole)
                                            .ThenInclude(x => x.Role).FirstOrDefaultAsync();
            if (appUser.Is_EmailVerified != true)
                return new ServiceResponse<LoginOtpResponse>(401, "Email not Verified, Please Verify your Email", null);

            var loginValid = await _context.UserAuthentications.Where(x => x.UserId == appUser.Id &&
                                                                          x.IsActive == true)
                                                              .ToListAsync();
            foreach (var login in loginValid)
            {
                login.IsActive = false;
            }

            var r = new Random();
            long otp = r.Next(100000, 199999);
            var newOtp = otp.ToString();
            UserAuthentication userAuthentication = new()
            {
                UserId = appUser.Id,
                OTP = newOtp,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ExpireTime = DateTime.UtcNow.AddMinutes(10)
            };
            _context.UpdateRange(loginValid);
            await _context.AddAsync(userAuthentication);
            await _context.SaveChangesAsync();

            TwilioClient.Init(_configuration.GetValue<string>("Twilio:AccountSID"),
                    _configuration.GetValue<string>("Twilio:AuthToken"));

            try
            {
                var messege = MessageResource.Create(
                    body: $"Your Login OTP for contractmint.io is {userAuthentication.OTP}",
                    from: _configuration.GetValue<string>("Twilio:TwilioPhoneNumber"),
                    to: $"{appUser.InternationalDailingCode}{appUser.ContactNumber}");
            }
            catch
            {

            }

            await _emailService.ResendVerifyOTPMail(appUser.EmailId, userAuthentication.OTP);

            var response = new LoginOtpResponse
            {
                EmailId = appUser.EmailId,
                LoginResponse = null
            };
            var endNumb = appUser.ContactNumber.ToString()[^2..];
            return new ServiceResponse<LoginOtpResponse>(StatusCodes.Status200OK,
                $"A One Time Password (OTP) has been sent to your mobile number xxxxxxxx{endNumb} and E-Mail", response);

        }




        public async Task<ServiceResponse<LoginResponse>> RefreshToken(string refreshToken)
        {
            var token = await _context.RefreshToken.Where(x => x.Token == refreshToken).FirstOrDefaultAsync();
            if (token is not null && DateTime.UtcNow <= token.Expires)
            {
                var appUser = await _context.User.Where(x => x.Id == token.UserId).Include(x => x.UserRole).ThenInclude(x => x.Role).FirstOrDefaultAsync();
                List<string> roles = appUser.UserRole.Select(x => x.Role.Name).ToList();
                var rt = GenerateRefreshToken();
                appUser.RefreshToken.Add(rt);
                _context.Remove(token);
                await _context.SaveChangesAsync();

                var res = new LoginResponse
                {
                    AccessToken = AccessToken(appUser, roles),
                    ExpiresIn = 86400,
                    RefreshToken = rt.Token,
                    Roles = roles,
                    TokenType = "Bearer",
                    Username = $"{appUser.EmailId}"
                };
                return new ServiceResponse<LoginResponse>(StatusCodes.Status200OK, "", res);
            }
            else
            {
                return new ServiceResponse<LoginResponse>(StatusCodes.Status401Unauthorized, "Invalid reset token", null);
            }
        }

        public async Task<ApiResponse> ForgotPassword(string email)
        {
            try
            {
                var appuser = await _context.User.Where(x => x.EmailId == email).FirstOrDefaultAsync();
                if (appuser != null)
                {
                    PasswordResetToken token = new()
                    {
                        Token = Guid.NewGuid().ToString(),
                        Username = email,
                        ExpiryDate = DateTime.UtcNow.AddHours(3)
                    };
                    await _context.PasswordResetToken.AddAsync(token);
                    await _context.SaveChangesAsync();
                    await _emailService.ForgotPasswordEmail(appuser.EmailId, $"{_configuration.GetValue<string>("Domain:Angular")}/reset?email={appuser.EmailId}&token={token.Token}");

                    return new ApiResponse("Password reset link sent to your email", 200);
                }
                return new ApiResponse("please enter a valid email", 400);
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message, 400);
            }
        }

        public async Task<ApiResponse> ResetPassword(string email, string token, string password)
        {
            var appuser = await _context.User.Where(x => x.EmailId == email && x.IsActive == false).FirstOrDefaultAsync();
            if (appuser != null)
            {
                var prt = await _context.PasswordResetToken.Where(x => x.Username == email && x.Token == token).FirstOrDefaultAsync();
                if (prt != null)
                {
                    appuser.Password = Encipher(password);
                    _context.Entry(appuser).State = EntityState.Modified;
                    _context.Remove(prt);
                    await _context.SaveChangesAsync();
                    return new ApiResponse("Password reset successful", 200);
                }
            }
            return new ApiResponse("Wrong reset link", 404);
        }


        public async Task<ApiResponse> VerifyEmail(string token)
        {
            var prt = await _context.PasswordResetToken.Where(x => x.Token == token).FirstOrDefaultAsync();
            if (prt != null)
            {
                var appuser = await _context.User.Where(x => x.EmailId == prt.Username).FirstOrDefaultAsync();
                if (appuser != null)
                {
                    appuser.Is_EmailVerified = true;
                    _context.Update(appuser);
                    _context.Remove(prt);
                    await _context.SaveChangesAsync();
                    return new ApiResponse("Email Verified  Successful", 200);
                }
                return new ApiResponse("Wrong reset link", 404);
            }
            return new ApiResponse("Wrong reset link", 400);
        }

        public async Task<ApiResponse> ChangePassword(string currentEmailId, ChangePasswordModel model)
        {
            var appuser = await _context.User.Where(x => x.EmailId == currentEmailId).FirstOrDefaultAsync();
            if (appuser is not null && (Decipher(appuser.Password) == model.CurrentPassword))
            {
                appuser.Password = Encipher(model.Password);
                _context.Entry(appuser).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return new ApiResponse("password has been changed successfully", 200);
            }
            else
            {
                return new ApiResponse("please enter a valid password", 400);
            }
        }


        private string AccessToken(User appUser, List<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JwtOptions:SecurityKey"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration.GetValue<string>("JwtOptions:Issuer"),
                Audience = _configuration.GetValue<string>("JwtOptions:Audience"),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, appUser.EmailId),
                    new Claim(ClaimTypes.NameIdentifier, $"{appUser.Id}"),

                    new Claim("roles", JsonConvert.SerializeObject(roles))
                }),
                Expires = DateTime.UtcNow.AddHours(9),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            roles.ForEach(x => tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, x)));
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static RefreshToken GenerateRefreshToken()
        {
#pragma warning disable SYSLIB0023 // Type or member is obsolete
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
#pragma warning restore SYSLIB0023 // Type or member is obsolete
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };
        }

        private static string Encipher(string Password)
        {
            string key = "abcdefghijklmnopqrstuvwxyz1234567890";
            byte[] bytesBuff = Encoding.Unicode.GetBytes(Password);
            using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new(key,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using MemoryStream mStream = new();
                using (CryptoStream cStream = new(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cStream.Write(bytesBuff, 0, bytesBuff.Length);
                    cStream.Close();
                }
                Password = Convert.ToBase64String(mStream.ToArray());
            }
            return Password;
        }

        private static string Decipher(string Password)
        {
            string key = "abcdefghijklmnopqrstuvwxyz1234567890";
            Password = Password.Replace(" ", "+");
            byte[] bytesBuff = Convert.FromBase64String(Password);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new(key,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using MemoryStream mStream = new();
                using (CryptoStream cStream = new(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cStream.Write(bytesBuff, 0, bytesBuff.Length);
                    cStream.Close();
                }
                Password = Encoding.Unicode.GetString(mStream.ToArray());
            }
            return Password;
        }

        public async Task<ApiResponse> ResendVerifyEmail(string email)
        {
            var user = _context.User.Where(x => x.EmailId == email).FirstOrDefault();
            var tok = _context.PasswordResetToken.Where(x => x.Username == email && x.ExpiryDate >= DateTime.UtcNow.AddMinutes(10)).FirstOrDefault();
            PasswordResetToken token = new();
            if (tok == null)
            {
                token.Token = Guid.NewGuid().ToString();
                token.Username = user.EmailId;
                token.ExpiryDate = DateTime.UtcNow.AddHours(3);
                await _context.PasswordResetToken.AddAsync(token);
            }
            else
            {
                token = tok;
            }
            await _context.SaveChangesAsync();
            await _emailService.VerifyRegistrationEmail($"{user.FullName}",
            user.EmailId, $"{_configuration.GetValue<string>("Domain:Angular")}/verify-email?email={user.EmailId}&token={token.Token}");
            return new ApiResponse("Verify Email Resent To " + email, 200);

        }
    }
}

