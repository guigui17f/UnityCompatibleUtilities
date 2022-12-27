using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GUIGUI17F
{
    public static class ExtensionMethods
    {
        private static readonly DateTime TimestampStartDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
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

        #region Transform

        /// <summary>
        /// resize a quad localScale to a target ratio by decrease x or y axis scale
        /// </summary>
        /// <param name="quad">target quad</param>
        /// <param name="ratio">width/height</param>
        public static void ResizeToFit(this Transform quad, float ratio)
        {
            Vector3 originScale = quad.localScale;
            float originRatio = originScale.x / originScale.y;
            if (originRatio > ratio)
            {
                //resize width
                float newWidth = originScale.y * ratio;
                quad.localScale = new Vector3(newWidth, originScale.y, originScale.z);
            }
            else if (originRatio < ratio)
            {
                //resize height
                float newHeight = originScale.x / ratio;
                quad.localScale = new Vector3(originScale.x, newHeight, originScale.z);
            }
        }

        /// <summary>
        /// resize a quad localScale to a target ratio by increase x or y axis scale
        /// </summary>
        /// <param name="quad">target quad</param>
        /// <param name="ratio">width/height</param>
        public static void ResizeToFill(this Transform quad, float ratio)
        {
            Vector3 originScale = quad.localScale;
            float originRatio = originScale.x / originScale.y;
            if (originRatio > ratio)
            {
                //resize height
                float newHeight = originScale.x / ratio;
                quad.localScale = new Vector3(originScale.x, newHeight, originScale.z);
            }
            else if (originRatio < ratio)
            {
                //resize width
                float newWidth = originScale.y * ratio;
                quad.localScale = new Vector3(newWidth, originScale.y, originScale.z);
            }
        }

        #endregion

        #region RectTransform

        /// <summary>
        /// resize a rect transform to a target ratio by decrease x or y axis size
        /// </summary>
        /// <param name="target">target rect transform</param>
        /// <param name="ratio">width/height</param>
        public static void ResizeToFit(this RectTransform target, float ratio)
        {
            Rect originSize = target.rect;
            float originRatio = originSize.width / originSize.height;
            if (originRatio > ratio)
            {
                //resize width
                float newWidth = originSize.height * ratio;
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            }
            else if (originRatio < ratio)
            {
                //resize height
                float newHeight = originSize.width / ratio;
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            }
        }

        /// <summary>
        /// resize a rect transform to a target ratio by increase x or y axis size
        /// </summary>
        /// <param name="target">target rect transform</param>
        /// <param name="ratio">width/height</param>
        public static void ResizeToFill(this RectTransform target, float ratio)
        {
            Rect originSize = target.rect;
            float originRatio = originSize.width / originSize.height;
            if (originRatio > ratio)
            {
                //resize height
                float newHeight = originSize.width / ratio;
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            }
            else if (originRatio < ratio)
            {
                //resize width
                float newWidth = originSize.height * ratio;
                target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            }
        }

        #endregion

        #region GameObject

        public static void DestroyAllChildren(this GameObject gameObject)
        {
            Transform transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// set layer to this gameObject and all children of it
        /// </summary>
        public static void SetLayerInHierarchy(this GameObject gameObject, int layer)
        {
            Stack<Transform> transformStack = new Stack<Transform>();
            transformStack.Push(gameObject.transform);
            while (transformStack.Count > 0)
            {
                Transform transform = transformStack.Pop();
                transform.gameObject.layer = layer;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transformStack.Push(transform.GetChild(i));
                }
            }
        }

        #endregion

        #region LayerMask

        public static bool ContainsLayer(this LayerMask mask, int layer)
        {
            return ((1 << layer) & mask) > 0;
        }

        #endregion
    }
}