using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Marketplace.Framework
{
    /// <summary>
    /// 值物件基底類別
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Value<T> where T : Value<T>
    {
        /// <summary>
        /// 用於存儲著當前類型T及其基類中的所有成員（字段和屬性）的資訊。
        /// </summary>
        [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
        private static readonly Member[] Members = GetMembers().ToArray();

        /// <summary>
        /// 比較兩個值是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            // 如果參數為null，則返回false。
            if (ReferenceEquals(null, other)) return false;

            // 如果參考位置相同，視為兩者相同，返回true。
            if (ReferenceEquals(this, other)) return true;

            //遍歷Members陣列，比較對象的每個成員的值。如果所有成員的值都相等，並且另一個對象的類型是T，則返回true；否則返回false。
            var members = Members;

            bool isSameType = other.GetType() == typeof(T);
            bool isValueEqual = Members
                .All(m =>
                {
                    // 取得other物件的值
                    var otherValue = m.GetValue(other);
                    // 取得this物件的值
                    var thisValue = m.GetValue(this);
                    // 如果是非字串的集合，則使用SequenceEqual方法比較兩個集合是否相等；否則使用Equals方法比較兩個值是否相等。
                    return m.IsNonStringEnumerable
                        ? GetEnumerableValues(otherValue).SequenceEqual(GetEnumerableValues(thisValue))
                        : (otherValue?.Equals(thisValue) ?? thisValue == null);
                });

            return isSameType && isValueEqual;
        }

        /// <summary>
        /// 取得雜湊碼
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // 遍歷Members陣列，取得每個成員的值，並計算每個值的雜湊碼的合併值。
            IEnumerable<object> objs = Members.Select(m =>
                    // 如果是非字串的集合，則使用CombineHashCodes方法計算集合中的每個值的雜湊碼的合併值；否則直接取得該值的雜湊碼。
                    m.IsNonStringEnumerable ? CombineHashCodes(GetEnumerableValues(m.GetValue(this))) : m.GetValue(this)
                );

            // 計算objs陣列中每個值的雜湊碼的合併值。
            return CombineHashCodes(objs);
        }

        /// <summary>
        /// 重寫==，使用Equals方法比較兩個值是否相等。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Value<T> left, Value<T> right) => Equals(left, right);

        /// <summary>
        /// 重寫!=，使用Equals方法比較兩個值是否相等，並將結果取反。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Value<T> left, Value<T> right) => !Equals(left, right);

        /// <summary>
        /// 覆寫ToString方法，返回類型名稱及每個成員的值。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Members.Length == 1)
            {
                // 如果只有一個成員，則直接取得該成員的值。
                var m = Members[0];
                var value = m.GetValue(this);
                return m.IsNonStringEnumerable
                    ? $"{string.Join("|", GetEnumerableValues(value))}"
                    : value.ToString();
            }

            // 如果有多個成員，則取得每個成員的值，並使用|分隔。
            var values = Members.Select(m =>
            {
                var value = m.GetValue(this);
                return m.IsNonStringEnumerable
                    ? $"{m.Name}:{string.Join("|", GetEnumerableValues(value))}"
                    : m.Type != typeof(string)
                        ? $"{m.Name}:{value}"
                        : value == null
                            ? $"{m.Name}:null"
                            : $"{m.Name}:\"{value}\"";
            });
            return $"{typeof(T).Name}[{string.Join("|", values)}]";
        }

        /// <summary>
        /// 取得類別成員
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Member> GetMembers()
        {
            // 取得類型T及其基類中的所有公共實例成員（字段和屬性）。
            var t = typeof(T);
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            // 遍歷所有成員，並將其封裝成Member物件。
            while (t != typeof(object))
            {
                if (t == null) continue;

                // 取得屬性
                foreach (var p in t.GetProperties(flags)) yield return new Member(p);

                // 取得欄位
                foreach (var f in t.GetFields(flags)) yield return new Member(f);

                // 取得基類，並繼續遍歷
                t = t.BaseType;
            }
        }

        /// <summary>
        /// 取得集合中的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static IEnumerable<object> GetEnumerableValues(object obj)
        {
            var enumerator = ((IEnumerable)obj).GetEnumerator();
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }

        /// <summary>
        /// 合併雜湊碼
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        private static int CombineHashCodes(IEnumerable<object> objs)
        {
            //計算過程中禁用整數溢出檢查
            //最終計算結果超出整數類型的表示範圍時，該值將循環回到該類型的最小值
            unchecked
            {
                // 常數 17 是一個通常被用作起始值的數字。它被認為是一個相對較小的質數，可以提供一定的隨機性，有助於減少碰撞的機會。
                // 常數 59 是一個相對較大的質數，可以提供更多的隨機性。
                // Aggregate：current是一個累計值，*59之後再加上下一個值的雜湊碼。
                return objs.Aggregate(17, (current, obj) => current * 59 + (obj?.GetHashCode() ?? 0));
            }
        }

        /// <summary>
        /// 類別成員
        /// </summary>
        private struct Member
        {
            /// <summary>
            /// 成員名稱
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// 取得成員值的委派
            /// </summary>
            public readonly Func<object, object> GetValue;

            /// <summary>
            /// 是否為非字串的集合
            /// </summary>
            public readonly bool IsNonStringEnumerable;

            /// <summary>
            /// 成員型別
            /// </summary>
            public readonly Type Type;

            public Member(MemberInfo info)
            {
                switch (info)
                {
                    case FieldInfo field:
                        Name = field.Name;
                        GetValue = obj => field.GetValue(obj);

                        IsNonStringEnumerable =
                            //可列舉型別（IEnumerable）
                            typeof(IEnumerable).IsAssignableFrom(field.FieldType)
                            //不是字串型別（typeof(string)）
                            && field.FieldType != typeof(string);

                        Type = field.FieldType;
                        break;
                    case PropertyInfo prop:
                        Name = prop.Name;
                        GetValue = obj => prop.GetValue(obj);

                        IsNonStringEnumerable =
                            // 可列舉型別（IEnumerable）
                            typeof(IEnumerable).IsAssignableFrom(prop.PropertyType)
                            // 不是字串型別（typeof(string)）
                            && prop.PropertyType != typeof(string);

                        Type = prop.PropertyType;
                        break;
                    default:
                        throw new ArgumentException("Member is not a field or property?!?!", info.Name);
                }
            }
        }
    }
}
