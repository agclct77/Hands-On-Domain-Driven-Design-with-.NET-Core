using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;

namespace Marketplace
{
    /// <summary>
    /// 固定貨幣查詢
    /// </summary>
    public class FixedCurrencyLookup : ICurrencyLookup
    {
        /// <summary>
        /// 固定貨幣清單
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
        /// 依據貨幣代碼查詢貨幣
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