using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class TestWindow : EditorWindow
{
    [MenuItem("Test/TestWindow")]
    public static void ShowWindow()
    {
        TestWindow window = GetWindow<TestWindow>();
        window.titleContent = new GUIContent("Test Window");
        window.Show();
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        Button button = new Button(OnClickButton) { text = "Action" };
        root.Add(button);
    }

    private void OnClickButton()
    {
    }
}