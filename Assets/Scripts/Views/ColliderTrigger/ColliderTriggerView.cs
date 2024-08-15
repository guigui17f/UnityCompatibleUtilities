using System;
using UnityEngine;

namespace GUIGUI17F
{
    [RequireComponent(typeof(Collider))]
    public class ColliderTriggerView : MonoBehaviour
    {
        public event Action<string, Collider, bool> OnTriggerEvent;
        public event Action<string, Collision, bool> OnCollisionEvent;

        public string TriggerId;
        public LayerMask TriggerLayers;
        public ColliderTriggerConfig[] TriggerConfigs;

        private void OnTriggerEnter(Collider other)
        {
            if (TriggerLayers.ContainsLayer(other.gameObject.layer))
            {
                OnTriggerEvent?.Invoke(TriggerId, other, true);
                TriggerBehaviour(ColliderTriggerType.TriggerEnter);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (TriggerLayers.ContainsLayer(other.gameObject.layer))
            {
                OnTriggerEvent?.Invoke(TriggerId, other, false);
                TriggerBehaviour(ColliderTriggerType.TriggerExit);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (TriggerLayers.ContainsLayer(collision.gameObject.layer))
            {
                OnCollisionEvent?.Invoke(TriggerId, collision, true);
                TriggerBehaviour(ColliderTriggerType.CollisionEnter);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (TriggerLayers.ContainsLayer(collision.gameObject.layer))
            {
                OnCollisionEvent?.Invoke(TriggerId, collision, false);
                TriggerBehaviour(ColliderTriggerType.CollisionExit);
            }
        }

        private void TriggerBehaviour(ColliderTriggerType triggerType)
        {
            for (int i = 0; i < TriggerConfigs.Length; i++)
            {
                var config = TriggerConfigs[i];
                if (config.TriggerType == triggerType)
                {
                    switch (config.TriggerBehaviour)
                    {
                        case ColliderTriggerBehaviour.ToggleGameObject:
                            config.ObjectReference.SetActive(config.BoolValue);
                            break;
                        case ColliderTriggerBehaviour.ToggleCollider:
                            config.ColliderReference.enabled = config.BoolValue;
                            break;
                        case ColliderTriggerBehaviour.ToggleBehaviour:
                            config.BehaviourReference.enabled = config.BoolValue;
                            break;
                    }
                }
            }
        }
    }
}