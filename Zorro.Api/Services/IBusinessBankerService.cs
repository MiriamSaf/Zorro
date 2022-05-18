using Zorro.Dal.Models;

namespace Zorro.Api.Services
{
    public interface IBusinessBankerService
    {
        Task<Guid> ProcessPayment(string merchantWalletId, string customerWalletId, decimal amount, Currency currencyType, string comment);
        Guid RefundPayment(Guid transactionId);
    }
}