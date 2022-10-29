using UnityEngine;
using UnityEditor;

namespace GUIGUI17F
{
    [CustomPropertyDrawer(typeof(NeedInterfaceAttribute))]
    public class NeedInterfaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label.text, "NeedInterface attribute can only be used with MonoBehaviours!");
                return;
            }
            NeedInterfaceAttribute interfaceAttribute = attribute as NeedInterfaceAttribute;
            EditorGUI.BeginProperty(position, label, property);
            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, interfaceAttribute.InterfaceType, true);
            EditorGUI.EndProperty();
        }
    }
}