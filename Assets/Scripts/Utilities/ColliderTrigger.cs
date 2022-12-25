using System;
using UnityEngine;

namespace GUIGUI17F
{
    public class ColliderTrigger : MonoBehaviour
    {
        public event Action<Collision> OnCollisionEnterEvent;
        public event Action<Collision> OnCollisionExitEvent;
        public event Action<Collider> OnTriggerEnterEvent;
        public event Action<Collider> OnTriggerExitEvent;
        
        public LayerMask InteractionLayer;

        private void OnCollisionEnter(Collision collision)
        {
            if (InteractionLayer.ContainsLayer(collision.gameObject.layer))
            {
                OnCollisionEnterEvent?.Invoke(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (InteractionLayer.ContainsLayer(collision.gameObject.layer))
            {
                OnCollisionExitEvent?.Invoke(collision);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (InteractionLayer.ContainsLayer(other.gameObject.layer))
            {
                OnTriggerEnterEvent?.Invoke(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (InteractionLayer.ContainsLayer(other.gameObject.layer))
            {
                OnTriggerExitEvent?.Invoke(other);
            }
        }
    }
}