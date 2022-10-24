using System;

namespace GUIGUI17F
{
    /// <summary>
    /// signal for events without parameters
    /// </summary>
    public class Signal
    {
        private event Action _event;
        
        public void AddListener(Action listener)
        {
            _event -= listener;
            _event += listener;
        }
        
        public void RemoveListener(Action listener)
        {
            _event -= listener;
        }
        
        public void ClearListeners()
        {
            _event = null;
        }
        
        public void Dispatch()
        {
            _event?.Invoke();
        }
    }

    /// <summary>
    /// signal for events with one parameter
    /// </summary>
    public class Signal<T>
    {
        private event Action<T> _event;
        
        public void AddListener(Action<T> listener)
        {
            _event -= listener;
            _event += listener;
        }
        
        public void RemoveListener(Action<T> listener)
        {
            _event -= listener;
        }
        
        public void ClearListeners()
        {
            _event = null;
        }
        
        public void Dispatch(T arg)
        {
            _event?.Invoke(arg);
        }
    }

    /// <summary>
    /// signal for events with two parameters
    /// </summary>
    public class Signal<T0, T1>
    {
        private event Action<T0, T1> _event;
        
        public void AddListener(Action<T0, T1> listener)
        {
            _event -= listener;
            _event += listener;
        }
        
        public void RemoveListener(Action<T0, T1> listener)
        {
            _event -= listener;
        }
        
        public void ClearListeners()
        {
            _event = null;
        }
        
        public void Dispatch(T0 arg0, T1 arg1)
        {
            _event?.Invoke(arg0, arg1);
        }
    }

    /// <summary>
    /// signal for events with three parameters
    /// </summary>
    public class Signal<T0, T1, T2>
    {
        private event Action<T0, T1, T2> _event;
        
        public void AddListener(Action<T0, T1, T2> listener)
        {
            _event -= listener;
            _event += listener;
        }
        
        public void RemoveListener(Action<T0, T1, T2> listener)
        {
            _event -= listener;
        }
        
        public void ClearListeners()
        {
            _event = null;
        }
        
        public void Dispatch(T0 arg0, T1 arg1, T2 arg2)
        {
            _event?.Invoke(arg0, arg1, arg2);
        }
    }

    /// <summary>
    /// signal for events with four parameters
    /// </summary>
    public class Signal<T0, T1, T2, T3>
    {
        private event Action<T0, T1, T2, T3> _event;
        
        public void AddListener(Action<T0, T1, T2, T3> listener)
        {
            _event -= listener;
            _event += listener;
        }
        
        public void RemoveListener(Action<T0, T1, T2, T3> listener)
        {
            _event -= listener;
        }
        
        public void ClearListeners()
        {
            _event = null;
        }
        
        public void Dispatch(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            _event?.Invoke(arg0, arg1, arg2, arg3);
        }
    }

    /// <summary>
    /// signal for events with five parameters
    /// </summary>
    public class Signal<T0, T1, T2, T3, T4>
    {
        private event Action<T0, T1, T2, T3, T4> _event;
        
        public void AddListener(Action<T0, T1, T2, T3, T4> listener)
        {
            _event -= listener;
            _event += listener;
        }
        
        public void RemoveListener(Action<T0, T1, T2, T3, T4> listener)
        {
            _event -= listener;
        }
        
        public void ClearListeners()
        {
            _event = null;
        }
        
        public void Dispatch(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _event?.Invoke(arg0, arg1, arg2, arg3, arg4);
        }
    }

    /// <summary>
    /// signal for events with six parameters
    /// </summary>
    public class Signal<T0, T1, T2, T3, T4, T5>
    {
        private event Action<T0, T1, T2, T3, T4, T5> _event;
        
        public void AddListener(Action<T0, T1, T2, T3, T4, T5> listener)
        {
            _event -= listener;
            _event += listener;
        }
        
        public void RemoveListener(Action<T0, T1, T2, T3, T4, T5> listener)
        {
            _event -= listener;
        }
        
        public void ClearListeners()
        {
            _event = null;
        }
        
        public void Dispatch(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _event?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5);
        }
    }
}
