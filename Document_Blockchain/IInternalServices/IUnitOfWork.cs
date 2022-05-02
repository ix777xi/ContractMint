using Document_Blockchain.Docusign_;

namespace Document_Blockchain.IInternalServices
{
    public interface IUnitOfWork
    {
        IAccountService AccountService { get; }

        IAdminServices AdminServices { get; }

        IEmailServices EmailServices { get; }

        IStorage Storage { get; }

        IUserServices UserServices { get; }

        IWebUserServices WebUserServices { get; }

        IDocusign Docusign { get; }

        IPaymentServices PaymentServices { get; }
    }
}
