namespace Zorro.Api.Exceptions
{
    public class WalletNotFoundException : Exception
    {
        public WalletNotFoundException()
        {
        }

        public WalletNotFoundException(string message)
            : base(message)
        {
        }
    }
}
