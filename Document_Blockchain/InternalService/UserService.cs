using Document_Blockchain.IInternalServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Document_Blockchain.InternalService
{


    public class UserService : IUserServices
    {
        private readonly IHttpContextAccessor _httpContext;
        public UserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public long UserID => long.Parse(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        public string Email => _httpContext.HttpContext.User.FindFirst(ClaimTypes.Email).Value;

        public string Name => _httpContext.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

        public int GroupID => Convert.ToInt32(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
}