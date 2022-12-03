using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GUIGUI17F
{
    /// <summary>
    /// used for debug memory leak
    /// </summary>
    public class ComponentReferenceMonitor
    {
        private const string LogTag = "ComponentReferenceMonitor";
        private static Dictionary<string, WeakReference> _referenceDictionary = new Dictionary<string, WeakReference>();
        private static List<string> _removeList = new List<string>();
        private static StringBuilder _stringBuilder = new StringBuilder();
        private static long _index;

        public static void AddRecord(Component component)
        {
            Transform current = component.transform;
            while (current != null)
            {
                _stringBuilder.Insert(0, '/');
                _stringBuilder.Insert(1, current.name);
                current = current.parent;
            }
            string path = _stringBuilder.ToString();
            _stringBuilder.Clear();
            _stringBuilder.Append("index:");
            _stringBuilder.Append(_index);
            _stringBuilder.Append(", component:");
            _stringBuilder.Append(component.GetType());
            _stringBuilder.Append(", path:");
            _stringBuilder.Append(path);
            _referenceDictionary.Add(_stringBuilder.ToString(), new WeakReference(component));
            _stringBuilder.Clear();
            _index++;
        }

        public static void PrintStatus()
        {
            foreach (KeyValuePair<string, WeakReference> pair in _referenceDictionary)
            {
                if (pair.Value.IsAlive)
                {
                    if (pair.Value.Target == null)
                    {
                        Debug.unityLogger.LogError(LogTag, $"{pair.Key} leak, target is null");
                    }
                    else
                    {
                        Component component = pair.Value.Target as Component;
                        if (component == null)
                        {
                            Debug.unityLogger.LogError(LogTag, $"{pair.Key} leak, component is null");
                        }
                        else if (component.gameObject == null)
                        {
                            Debug.unityLogger.LogError(LogTag, $"{pair.Key} leak, gameObject is null");
                        }
                    }
                }
                else
                {
                    _removeList.Add(pair.Key);
                }
            }
            foreach (string item in _removeList)
            {
                _referenceDictionary.Remove(item);
            }
            _removeList.Clear();
        }
    }
}