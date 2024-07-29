using System;
using System.Collections.Generic;

namespace GUIGUI17F
{
    public class GenericObjectPool
    {
        private const int DEFAULT_CAPACITY = 100;

        private static Dictionary<Type, Queue<object>> _poolDictionary = new();

        public static T Get<T>() where T : class, new()
        {
            if (_poolDictionary.TryGetValue(typeof(T), out var queue))
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue() as T;
                }
                else
                {
                    return new T();
                }
            }
            else
            {
                return new T();
            }
        }

        public static void Recycle<T>(T obj) where T : class, new()
        {
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
            var type = typeof(T);
            if (_poolDictionary.TryGetValue(type, out var queue))
            {
                if (queue.Count < DEFAULT_CAPACITY && !queue.Contains(obj))
                {
                    queue.Enqueue(obj);
                }
            }
            else
            {
                var newQueue = new Queue<object>();
                newQueue.Enqueue(obj);
                _poolDictionary.Add(type, newQueue);
            }
        }
    }
}