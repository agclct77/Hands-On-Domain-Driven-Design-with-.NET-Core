using System;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class Money : Value<Money>
    {
        /// <summary>
        /// 建立一個新的Money物件，輸入金額、貨幣代碼、貨幣查詢物件
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currency"></param>
        /// <param name="currencyLookup"></param>
        /// <returns></returns>
        public static Money FromDecimal(decimal amount, string currency, ICurrencyLookup currencyLookup) =>
            new Money(amount, currency, currencyLookup);

        /// <summary>
        /// 建立一個新的Money物件，輸入字串格式的金額、貨幣代碼、貨幣查詢物件
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currency"></param>
        /// <param name="currencyLookup"></param>
        /// <returns></returns>
        public static Money FromString(string amount, string currency, ICurrencyLookup currencyLookup) =>
            new Money(decimal.Parse(amount), currency, currencyLookup);

        /// <summary>
        /// (protected)建構子，建立一個新的Money物件，輸入金額、貨幣代碼、貨幣查詢物件
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currencyCode"></param>
        /// <param name="currencyLookup"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected Money(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
        {
            // 如果貨幣代碼為空或null，則拋出ArgumentNullException
            if (string.IsNullOrEmpty(currencyCode))
                throw new ArgumentNullException(
                    nameof(currencyCode), "Currency code must be specified");
            
            // 如果找不到對應的貨幣代碼，則拋出ArgumentException
            var currency = currencyLookup.FindCurrency(currencyCode);
            if (!currency.InUse)
                throw new ArgumentException($"Currency {currencyCode} is not valid");
            
            // 如果金額小數點位數大於貨幣的小數點位數，則拋出ArgumentOutOfRangeException
            if (decimal.Round(amount, currency.DecimalPlaces) != amount)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    $"Amount in {currencyCode} cannot have more than {currency.DecimalPlaces} decimals");

            Amount = amount;
            Currency = currency;
        }

        /// <summary>
        /// (protected)建構子，建立一個新的Money物件，輸入金額及貨幣
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currency"></param>
        protected Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        /// <summary>
        /// 金額
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// 貨幣
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Money的相加
        /// </summary>
        /// <param name="summand"></param>
        /// <returns></returns>
        /// <exception cref="CurrencyMismatchException"></exception>
        public Money Add(Money summand)
        {
            // 如果貨幣代碼不同，則拋出CurrencyMismatchException
            if (Currency != summand.Currency)
                throw new CurrencyMismatchException(
                    "Cannot sum amounts with different currencies");

            // 建立一個新的Money物件，並回傳
            return new Money(Amount + summand.Amount, Currency);
        }

        /// <summary>
        /// Money的相減
        /// </summary>
        /// <param name="subtrahend"></param>
        /// <returns></returns>
        /// <exception cref="CurrencyMismatchException"></exception>
        public Money Subtract(Money subtrahend)
        {
            // 如果貨幣代碼不同，則拋出CurrencyMismatchException
            if (Currency != subtrahend.Currency)
                throw new CurrencyMismatchException(
                    "Cannot subtract amounts with different currencies");

            // 建立一個新的Money物件，並回傳
            return new Money(Amount - subtrahend.Amount, Currency);
        }

        /// <summary>
        /// 覆寫加法運算子，讓Money物件可以直接相加。
        /// </summary>
        /// <param name="summand1"></param>
        /// <param name="summand2"></param>
        /// <returns></returns>
        public static Money operator +(Money summand1, Money summand2) =>
            summand1.Add(summand2);

        /// <summary>
        /// 覆寫減法運算子，讓Money物件可以直接相減。
        /// </summary>
        /// <param name="minuend"></param>
        /// <param name="subtrahend"></param>
        /// <returns></returns>
        public static Money operator -(Money minuend, Money subtrahend) =>
            minuend.Subtract(subtrahend);

        public override string ToString() => $"{Currency.CurrencyCode} {Amount}";
    }

    public class CurrencyMismatchException : Exception
    {
        public CurrencyMismatchException(string message) : base(message)
        {
        }
    }
}