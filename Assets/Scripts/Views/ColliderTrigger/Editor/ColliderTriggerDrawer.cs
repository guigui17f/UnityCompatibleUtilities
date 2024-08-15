using UnityEditor;
using UnityEngine;

namespace GUIGUI17F
{
    [CustomPropertyDrawer(typeof(ColliderTriggerConfig))]
    public class ColliderTriggerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            var verticalOffset = lineHeight + spacing;
            var triggerBehaviourProperty = property.FindPropertyRelative("TriggerBehaviour");
            var triggerBehaviourValue = (ColliderTriggerBehaviour)triggerBehaviourProperty.intValue;

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineHeight), property.FindPropertyRelative("TriggerType"));
            EditorGUI.PropertyField(new Rect(position.x, position.y + verticalOffset, position.width, lineHeight), triggerBehaviourProperty);
            switch (triggerBehaviourValue)
            {
                case ColliderTriggerBehaviour.ToggleGameObject:
                    EditorGUI.PropertyField(new Rect(position.x, position.y + verticalOffset * 2f, position.width, lineHeight), property.FindPropertyRelative("ObjectReference"));
                    EditorGUI.PropertyField(new Rect(position.x, position.y + verticalOffset * 3f, position.width, lineHeight), property.FindPropertyRelative("BoolValue"), new GUIContent("Enable"));
                    break;
                case ColliderTriggerBehaviour.ToggleCollider:
                    EditorGUI.PropertyField(new Rect(position.x, position.y + verticalOffset * 2f, position.width, lineHeight), property.FindPropertyRelative("ColliderReference"));
                    EditorGUI.PropertyField(new Rect(position.x, position.y + verticalOffset * 3f, position.width, lineHeight), property.FindPropertyRelative("BoolValue"), new GUIContent("Enable"));
                    break;
                case ColliderTriggerBehaviour.ToggleBehaviour:
                    EditorGUI.PropertyField(new Rect(position.x, position.y + verticalOffset * 2f, position.width, lineHeight), property.FindPropertyRelative("BehaviourReference"));
                    EditorGUI.PropertyField(new Rect(position.x, position.y + verticalOffset * 3f, position.width, lineHeight), property.FindPropertyRelative("BoolValue"), new GUIContent("Enable"));
                    break;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var verticalOffset = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var triggerBehaviourValue = (ColliderTriggerBehaviour)property.FindPropertyRelative("TriggerBehaviour").intValue;
            if (triggerBehaviourValue == ColliderTriggerBehaviour.None)
            {
                return verticalOffset * 2f + 5f;
            }
            else
            {
                return verticalOffset * 4f + 5f;
            }
        }
    }
}