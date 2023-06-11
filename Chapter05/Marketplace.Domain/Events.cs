using System;

namespace Marketplace.Domain
{
    /// <summary>
    /// 事件
    /// </summary>
    public static class Events
    {
        /// <summary>
        /// 廣告已建立
        /// </summary>
        public class ClassifiedAdCreated
        {
            public Guid Id { get; set; }
            public Guid OwnerId { get; set; }
        }

        /// <summary>
        /// 廣告標題已變更
        /// </summary>
        public class ClassifiedAdTitleChanged
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }

        /// <summary>
        /// 廣告內容已變更
        /// </summary>
        public class ClassifiedAdTextUpdated
        {
            public Guid Id { get; set; }
            public string AdText { get; set; }
        }

        /// <summary>
        /// 廣告價格已變更
        /// </summary>
        public class ClassifiedAdPriceUpdated
        {
            public Guid Id { get; set; }
            public decimal Price { get; set; }
            public string CurrencyCode { get; set; }
        }

        /// <summary>
        /// 廣告已送審
        /// </summary>
        public class ClassidiedAdSentForReview
        {
            public Guid Id { get; set; }
        }
    }
}