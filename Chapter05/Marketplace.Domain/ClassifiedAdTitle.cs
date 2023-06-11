using System;
using System.Text.RegularExpressions;
using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class ClassifiedAdTitle : Value<ClassifiedAdTitle>
    {
        /// <summary>
        /// (靜態工廠方法)從字串建立ClassifiedAdTitle物件
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static ClassifiedAdTitle FromString(string title)
        {
            CheckValidity(title);
            return new ClassifiedAdTitle(title);
        }

        /// <summary>
        /// (靜態工廠方法)從HTML標題建立ClassifiedAdTitle物件
        /// </summary>
        /// <param name="htmlTitle"></param>
        /// <returns></returns>
        public static ClassifiedAdTitle FromHtml(string htmlTitle)
        {
            // 將<i>、</i>、<b>、</b>標籤替換成Markdown格式
            var supportedTagsReplaced = htmlTitle
                .Replace("<i>", "*")
                .Replace("</i>", "*")
                .Replace("<b>", "**")
                .Replace("</b>", "**");

            // 移除所有的HTML標籤
            var value = Regex.Replace(supportedTagsReplaced, "<.*?>", string.Empty);

            // 檢查標題是否合法
            CheckValidity(value);

            // 實例化ClassifiedAdTitle物件
            return new ClassifiedAdTitle(value);
        }

        /// <summary>
        /// 標題的值
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// (internal)建構子，建立一個新的ClassifiedAdTitle物件，輸入標題
        /// </summary>
        /// <param name="value"></param>
        internal ClassifiedAdTitle(string value) => Value = value;

        /// <summary>
        /// (implicit)隱含轉換，將ClassifiedAdTitle物件轉換成string
        /// </summary>
        /// <param name="title"></param>
        public static implicit operator string(ClassifiedAdTitle title) =>
            title.Value;

        /// <summary>
        /// 檢查標題是否合法
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static void CheckValidity(string value)
        {
            if (value.Length > 100)
                throw new ArgumentOutOfRangeException(
                    "Title cannot be longer that 100 characters",
                    nameof(value));
        }
    }
}