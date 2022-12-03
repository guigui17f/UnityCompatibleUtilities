using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace GUIGUI17F
{
    public static class ExtensionMethods
    {
        private static readonly DateTime TimestampStartDate = new DateTime(1970, 1, 1, 0, 0, 0,0);
        private static StringBuilder _stringBuilder = new StringBuilder();

        #region Array

        public static bool ElementsEqual<T>(this T[] array0, T[] array1) where T : IEquatable<T>
        {
            if (array0 == array1)
            {
                return true;
            }
            if (array0 == null || array1 == null)
            {
                return false;
            }
            if (array0.Length != array1.Length)
            {
                return false;
            }
            for (int i = 0; i < array0.Length; i++)
            {
                if (!array0[i].Equals(array1[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static T[] SubArray<T>(this T[] sourceArray, int startIndex)
        {
            if (sourceArray == null)
            {
                return null;
            }
            startIndex = Mathf.Clamp(startIndex, 0, sourceArray.Length - 1);
            T[] resultArray = new T[sourceArray.Length - startIndex];
            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = sourceArray[startIndex + i];
            }
            return resultArray;
        }

        public static T[] SubArray<T>(this T[] sourceArray, int startIndex, int count)
        {
            if (sourceArray == null)
            {
                return null;
            }
            startIndex = Mathf.Clamp(startIndex, 0, sourceArray.Length - 1);
            count = Mathf.Clamp(count, 1, sourceArray.Length - startIndex);
            T[] resultArray = new T[count];
            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = sourceArray[startIndex + i];
            }
            return resultArray;
        }

        public static string ToSingleString<T>(this T[] sourceArray, string separator) where T : IConvertible
        {
            if (sourceArray == null)
            {
                return null;
            }
            if (sourceArray.Length == 0)
            {
                return string.Empty;
            }
            _stringBuilder.Clear();
            for (int i = 0; i < sourceArray.Length; i++)
            {
                _stringBuilder.Append(sourceArray[i].ToString(CultureInfo.InvariantCulture));
                _stringBuilder.Append(separator);
            }
            _stringBuilder.Remove(_stringBuilder.Length - separator.Length, separator.Length);
            string result = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return result;
        }

        #endregion

        #region List

        public static string ToSingleString<T>(this List<T> sourceList, string separator) where T : IConvertible
        {
            if (sourceList == null)
            {
                return null;
            }
            if (sourceList.Count == 0)
            {
                return string.Empty;
            }
            _stringBuilder.Clear();
            foreach (T item in sourceList)
            {
                _stringBuilder.Append(item.ToString(CultureInfo.InvariantCulture));
                _stringBuilder.Append(separator);
            }
            _stringBuilder.Remove(_stringBuilder.Length - separator.Length, separator.Length);
            string result = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return result;
        }

        #endregion

        #region DateTime

        /// <summary>
        /// get unix timestamp in milliseconds
        /// </summary>
        public static long ToTimestamp(this DateTime dateTime, bool isLocalTime)
        {
            if (isLocalTime)
            {
                dateTime = dateTime.ToUniversalTime();
            }
            return Convert.ToInt64((dateTime - TimestampStartDate).TotalMilliseconds);
        }

        #endregion

        #region long

        /// <summary>
        /// get DateTime from unix timestamp in milliseconds
        /// </summary>
        public static DateTime ToDateTime(this long timestamp, bool toLocalTime)
        {
            DateTime dateTime = TimestampStartDate.AddMilliseconds(timestamp);
            if (toLocalTime)
            {
                dateTime = dateTime.ToLocalTime();
            }
            return dateTime;
        }

        #endregion
    }
}