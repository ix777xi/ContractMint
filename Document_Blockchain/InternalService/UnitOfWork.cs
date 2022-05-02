using Document_Blockchain.Docusign_;
using Document_Blockchain.Entities;
using Document_Blockchain.IInternalServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Document_Blockchain.InternalService
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailServices _emailServices;
        private readonly IStorage _storage;
        private readonly IUserServices _userServices;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BlockChainDbContext _context;

        public UnitOfWork(IConfiguration configuration,
                          IEmailServices emailServices,
                          IStorage storage,
                          IUserServices userServices,
                          IWebHostEnvironment webHostEnvironment,
                          IHttpContextAccessor httpContextAccessor,
                          BlockChainDbContext context)
        {
            _configuration = configuration;
            _emailServices = emailServices;
            _storage = storage;
            _userServices = userServices;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public IAccountService AccountService =>
            new AccountService(_configuration, _emailServices, _context);

        public IAdminServices AdminServices =>
            new AdminServices(_storage, _userServices, _emailServices, _context);

        public IEmailServices EmailServices =>
            new EmailServices(_configuration, _webHostEnvironment);

        public IStorage Storage =>
            new Storage(_configuration);

        public IUserServices UserServices =>
            new UserService(_httpContextAccessor);

        public IWebUserServices WebUserServices =>
            new WebUserServices(_userServices, _storage, _emailServices, _context);

        public IDocusign Docusign =>
            new Docusign(_configuration, _userServices, _storage, _context);

        public IPaymentServices PaymentServices =>
            new PaymentServices(_context, _userServices, _configuration);
    }
}
