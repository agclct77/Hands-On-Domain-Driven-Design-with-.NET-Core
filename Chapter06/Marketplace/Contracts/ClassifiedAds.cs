using System;

namespace Marketplace.Contracts
{
    /// <summary>
    ///    This is the contract for the classified ads bounded context.
    /// </summary>
    public static class ClassifiedAds
    {
        /// <summary>
        ///   This is the version 1 of the contract.
        /// </summary>
        public static class V1
        {
            /// <summary>
            ///   This is the command to create a new classified ad.
            /// </summary>
            public class Create
            {
                public Guid Id { get; set; }
                public Guid OwnerId { get; set; }
            }

            /// <summary>
            ///  This is the command to set the title of a classified ad.
            /// </summary>
            public class SetTitle
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
            }

            /// <summary>
            /// This is the command to update the text of a classified ad.
            /// </summary>
            public class UpdateText
            {
                public Guid Id { get; set; }
                public string Text { get; set; }
            }

            /// <summary>
            /// This is the command to update the price of a classified ad.
            /// </summary>
            public class UpdatePrice
            {
                public Guid Id { get; set; }
                public decimal Price { get; set; }
                public string Currency { get; set; }
            }

            /// <summary>
            /// This is the command to publish a classified ad.
            /// </summary>
            public class RequestToPublish
            {
                public Guid Id { get; set; }
            }
        }
    }
}