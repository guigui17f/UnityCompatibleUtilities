using System;
using UnityEngine;

namespace GUIGUI17F
{
    [Serializable]
    public class ColliderTriggerConfig
    {
        public ColliderTriggerType TriggerType;
        public ColliderTriggerBehaviour TriggerBehaviour;
        public GameObject ObjectReference;
        public Collider ColliderReference;
        public Behaviour BehaviourReference;
        public bool BoolValue;
    }
}