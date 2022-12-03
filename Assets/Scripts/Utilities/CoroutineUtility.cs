using System.Collections;
using UnityEngine;

namespace GUIGUI17F
{
    public class CoroutineUtility : MonoBehaviour
    {
        public static CoroutineUtility Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("CoroutineUtility").AddComponent<CoroutineUtility>();
                }
                return _instance;
            }
        }
        
        private static CoroutineUtility _instance;
        
        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public static Coroutine StartMonoCoroutine(IEnumerator enumerator)
        {
            return Instance.StartCoroutine(enumerator);
        }

        public static void StopMonoCoroutine(Coroutine coroutine)
        {
            Instance.StopCoroutine(coroutine);
        }

        public static void StopAllMonoCoroutines()
        {
            Instance.StopAllCoroutines();
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}