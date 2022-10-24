using System;
using UnityEditor;
using UnityEngine;

namespace GUIGUI17F
{
    [CustomPropertyDrawer(typeof(StepRangeAttribute))]
    public class StepRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            StepRangeAttribute stepRange = attribute as StepRangeAttribute;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                int multiplier = Convert.ToInt32(property.floatValue / stepRange.Step);
                property.floatValue = stepRange.Step * multiplier;
                EditorGUI.Slider(position, property, stepRange.Min, stepRange.Max, label);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.IntSlider(position, property, (int)stepRange.Min, (int)stepRange.Max, label);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "use range with float or int.");
            }
        }
    }
}