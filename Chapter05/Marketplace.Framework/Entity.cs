using System;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Framework
{
    /// <summary>
    /// 實體
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class Entity<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// 事件列表
        /// </summary>
        private readonly List<object> _events;

        /// <summary>
        ///初始化事件列表
        /// </summary>
        protected Entity() => _events = new List<object>();

        /// <summary>
        /// 事件觸發
        /// </summary>
        /// <param name="event"></param>
        protected void Apply(object @event)
        {
            // 呼叫When方法，When方法由子類決定實做項目。
            When(@event);
            
            // 確保狀態正確
            EnsureValidState();

            // 將事件加入事件列表
            _events.Add(@event);
        }

        protected abstract void When(object @event);

        /// <summary>
        /// 檢索事件列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetChanges() => _events.AsEnumerable();

        /// <summary>
        /// 清除事件列表
        /// </summary>
        public void ClearChanges() => _events.Clear();
        
        /// <summary>
        /// 確保狀態正確
        /// </summary>
        protected abstract void EnsureValidState();
    }
}