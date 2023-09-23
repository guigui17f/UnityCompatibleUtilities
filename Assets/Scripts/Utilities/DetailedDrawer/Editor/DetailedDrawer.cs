using UnityEngine;
using UnityEditor;

namespace GUIGUI17F
{
    [CustomPropertyDrawer(typeof(DetailedAttribute), true)]
    public class DetailedDrawer : PropertyDrawer
    {
        // Cached scriptable object editor
        private Editor _editor;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw label
            EditorGUI.PropertyField(position, property, label, true);

            // Draw foldout arrow
            if (property.objectReferenceValue != null)
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
            }

            // Draw foldout properties
            if (property.isExpanded)
            {
                // Make child fields be indented
                EditorGUI.indentLevel++;

                // background
                GUILayout.BeginVertical("box");

                if (_editor == null)
                {
                    Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _editor);
                }

                // Draw object properties
                EditorGUI.BeginChangeCheck();
                if (_editor != null)
                {
                    _editor.OnInspectorGUI();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }

                GUILayout.EndVertical();

                // Set indent back to what it was
                EditorGUI.indentLevel--;
            }
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class UnityObjectEditor : Editor
    {
    }
}