using Zorro.WebApplication.Dtos;

namespace Zorro.WebApplication.Data
{
    public interface ITransactionSampleDataService
    {
        IEnumerable<TransactionDto> GetData();
    }

    public class TransactionSampleDataService : ITransactionSampleDataService
    {
        public IEnumerable<TransactionDto> GetData()
        {
            var transactions = new List<TransactionDto>();
            transactions.Add(new TransactionDto()
            {
                Id = new Guid(),
                Date = DateTime.Now,
                Description = "Shoes",
                Amount = 129.99M
            });
            transactions.Add(new TransactionDto()
            {
                Id = new Guid(),
                Date = DateTime.Now,
                Description = "Takeaway Food",
                Amount = 18.90M
            });
            transactions.Add(new TransactionDto()
            {
                Id = new Guid(),
                Date = DateTime.Now,
                Description = "Bills Pharmacy",
                Amount = 23.95M
            });
            transactions.Add(new TransactionDto()
            {
                Id = new Guid(),
                Date = DateTime.Now,
                Description = "Gym membership",
                Amount = 59.90M
            });

            return transactions;
        }
    }
}
