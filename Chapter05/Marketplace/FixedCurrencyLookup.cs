using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;

namespace Marketplace
{
    /// <summary>
    /// �T�w�f���d��
    /// </summary>
    public class FixedCurrencyLookup : ICurrencyLookup
    {
        /// <summary>
        /// �T�w�f���M��
        /// </summary>
        private static readonly IEnumerable<Currency> _currencies =
            new[]
            {
                new Currency
                {
                    CurrencyCode = "EUR",
                    DecimalPlaces = 2,
                    InUse = true
                },
                new Currency
                {
                    CurrencyCode = "USD",
                    DecimalPlaces = 2,
                    InUse = true
                }
            };

        /// <summary>
        /// �̾ڳf���N�X�d�߳f��
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public Currency FindCurrency(string currencyCode)
        {
            var currency = _currencies.FirstOrDefault(x => x.CurrencyCode == currencyCode);
            return currency ?? Currency.None;
        }
    }
}