using Zorro.Dal.Models;

namespace Zorro.Api.Services
{
    public interface IBusinessBankerService
    {
        Guid ProcessPayment(string merchantWalletId, string customerWalletId, decimal amount, Currency currencyType);
        Guid RefundPayment(Guid transactionId);
        decimal ConvertCurrencyWithFee(decimal amount, Currency currencyType);
        Wallet GetWalletByApiKey(string apiKey);
    }
}
