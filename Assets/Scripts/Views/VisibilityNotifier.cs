using UnityEngine;
using System;

namespace GUIGUI17F
{
    /// <summary>
    /// renderer visibility notifier
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class VisibilityNotifier : MonoBehaviour
    {
        /// <summary>
        /// parameter: is visible
        /// </summary>
        public event Action<bool> OnVisibilityChanged;

        public void OnBecameVisible()
        {
            OnVisibilityChanged?.Invoke(true);
        }
        
        public void OnBecameInvisible()
        {
            OnVisibilityChanged?.Invoke(false);
        }
    }
}