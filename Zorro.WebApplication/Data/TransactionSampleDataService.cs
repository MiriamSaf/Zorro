using Zorro.WebApplication.Dtos;

namespace Zorro.WebApplication.Data
{
    public interface ITransactionSampleDataService
    {
        IEnumerable<TransferRequestDto> GetData();
    }

    public class TransactionSampleDataService : ITransactionSampleDataService
    {
        public IEnumerable<TransferRequestDto> GetData()
        {
            var transactions = new List<TransferRequestDto>();
            transactions.Add(new TransferRequestDto()
            {
                
                Description = "Shoes",
                Amount = 129.99M
            });
            transactions.Add(new TransferRequestDto()
            {
                
                Description = "Takeaway Food",
                Amount = 18.90M
            });
            transactions.Add(new TransferRequestDto()
            {
                
                Description = "Bills Pharmacy",
                Amount = 23.95M
            });
            transactions.Add(new TransferRequestDto()
            {
                
                Description = "Gym membership",
                Amount = 59.90M
            });

            return transactions;
        }
    }
}
