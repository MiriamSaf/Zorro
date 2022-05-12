using Zorro.Dal.Models;

namespace Zorro.Api.Services
{
    public static class CurrencyRateMap
    {
        public static decimal GetConversionRate(Currency currency) => currency switch
        {
            (Currency.Aud) => 1,
            (Currency.Euro) => 1.51708M,
            (Currency.Cad) => 1.11747M,
            (Currency.Franc) => 1.46056M,
            (Currency.Pound) => 1.77892M,
            (Currency.Usd) => 1.45397M,
            (Currency.Yen) => 0.0113410M,
            _ => throw new InvalidOperationException($"{currency} isn't a supported currency")
        };
    }
}
