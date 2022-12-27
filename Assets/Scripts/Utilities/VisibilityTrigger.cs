using UnityEngine;
using System;

namespace GUIGUI17F
{
    /// <summary>
    /// renderer visibility trigger
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class VisibilityTrigger : MonoBehaviour
    {
        /// <summary>
        /// arg: is visible
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