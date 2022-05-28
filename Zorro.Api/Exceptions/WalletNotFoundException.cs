namespace Zorro.Api.Exceptions
{
    public abstract class AbstractZorroException : Exception
    {
        public AbstractZorroException() { }
        public AbstractZorroException(string message)
            : base(message)
        {
        }
    }

    public class WalletNotFoundException : AbstractZorroException
    {
        public WalletNotFoundException()
        {
        }

        public WalletNotFoundException(string message)
            : base(message)
        {
        }
    }

    public class InvalidCurrencyTypeException : AbstractZorroException
    {
        public InvalidCurrencyTypeException()
        {
        }

        public InvalidCurrencyTypeException(string message) : base(message)
        {
        }
    }

    public class InvalidAmountException : AbstractZorroException
    {
        public InvalidAmountException()
        {
        }

        public InvalidAmountException(string message) : base(message)
        {
        }
    }

    public class InsufficientFundsException : AbstractZorroException
    {
        public InsufficientFundsException()
        {
        }

        public InsufficientFundsException(string message) : base(message)
        {
        }
    }
}
