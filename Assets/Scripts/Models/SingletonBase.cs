using System;

namespace GUIGUI17F
{
    public class SingletonBase<T> : IDisposable where T : class, new()
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }

        protected static T _instance;

        protected SingletonBase()
        {
        }

        public virtual void Dispose()
        {
            _instance = null;
        }
    }
}