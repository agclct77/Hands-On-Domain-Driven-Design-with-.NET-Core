using Marketplace.Framework;

namespace Marketplace.Domain
{
    /// <summary>
    /// 貨幣查詢介面
    /// </summary>
    public interface ICurrencyLookup
    {
        /// <summary>
        /// 依據貨幣代碼查詢貨幣
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        Currency FindCurrency(string currencyCode);
    }

    /// <summary>
    /// 貨幣
    /// </summary>
    public class Currency : Value<Currency>
    {
        public string CurrencyCode { get; set; }
        public bool InUse { get; set; }
        public int DecimalPlaces { get; set; }

        /// <summary>
        /// 無貨幣
        /// </summary>
        public static Currency None = new Currency {InUse = false};
    }
}