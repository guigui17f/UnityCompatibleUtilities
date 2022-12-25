using System.Collections.Generic;
using UnityEngine;

namespace GUIGUI17F
{
    public class RandomList<T>
    {
        public int TotalCount { get; private set; }
        public int LeftCount => _list.Count;

        private IEnumerable<T> _origin;
        private List<T> _list;

        public RandomList(IEnumerable<T> origin)
        {
            _origin = origin;
            _list = new List<T>(_origin);
            TotalCount = _list.Count;
        }
        
        public T GetRandom()
        {
            if (_list.Count <= 0)
            {
                Reset();
            }
            int index = Random.Range(0, _list.Count);
            T random = _list[index];
            _list.RemoveAt(index);
            return random;
        }

        public void Reset()
        {
            _list.Clear();
            _list.AddRange(_origin);
        }
    }
}