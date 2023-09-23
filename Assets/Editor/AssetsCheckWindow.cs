using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GUIGUI17F
{
    public class AssetsCheckWindow : EditorWindow
    {
        private TextField _pathField;
        private ObjectField _textureField;
        private ObjectField _shaderField;
        private LayerField _layerField;
        private Label _label;

        private MethodInfo _clearConsoleMethod;
        private bool _isRunning;

        [MenuItem("Tools/Check Assets")]
        private static void ShowWindow()
        {
            var window = GetWindow<AssetsCheckWindow>();
            window.titleContent = new GUIContent("AssetsCheckWindow");
        }

        private void CreateGUI()
        {
            _clearConsoleMethod = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.LogEntries").GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);

            StyleLength margin = new StyleLength(10);
            VisualElement root = rootVisualElement;

            _pathField = new TextField("Search Path:") { value = "Assets" };
            root.Add(SetMargin(_pathField, margin));

            VisualElement container = new VisualElement();
            _textureField = CreateObjectField(typeof(Texture), margin);
            container.Add(_textureField);

            container.Add(CreateButton("find texture references", OnClickFindTextureReferences));
            root.Add(SetMargin(container, margin));

            container = new VisualElement();
            _shaderField = CreateObjectField(typeof(Shader), margin);
            container.Add(_shaderField);

            container.Add(CreateButton("find shader references", OnClickFindShaderReferences));
            root.Add(SetMargin(container, margin));

            container = new VisualElement();
            _layerField = new LayerField();
            _layerField.style.marginBottom = margin;
            container.Add(_layerField);

            container.Add(CreateButton("find layer usage", OnClickFindLayerUsage));
            root.Add(SetMargin(container, margin));

            root.Add(SetMargin(CreateButton("check shader errors", OnClickCheckShaderErrors), margin));
            root.Add(SetMargin(CreateButton("check script and prefab missing", OnClickCheckScriptAndPrefabMissing), margin));

            _label = new Label();
            root.Add(SetMargin(_label, margin));
        }

        private VisualElement SetMargin(VisualElement target, StyleLength length)
        {
            target.style.marginLeft = length;
            target.style.marginRight = length;
            target.style.marginTop = length;
            target.style.marginBottom = length;
            return target;
        }

        private ObjectField CreateObjectField(Type objectType, StyleLength bottomMargin)
        {
            ObjectField field = new ObjectField();
            field.objectType = objectType;
            field.style.marginBottom = bottomMargin;
            return field;
        }

        private Button CreateButton(string text, Action clickCallback)
        {
            Button button = new Button();
            button.text = text;
            button.clicked += clickCallback;
            return button;
        }

        private async void OnClickCheckScriptAndPrefabMissing()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _clearConsoleMethod.Invoke(null, null);

                string[] guids = AssetDatabase.FindAssets("t:prefab", new[] { _pathField.value });
                int feedbackInterval = Mathf.Max(guids.Length / 10, 1);
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    GameObject go = PrefabUtility.LoadPrefabContents(path);
                    MonoBehaviour[] scripts = go.GetComponentsInChildren<MonoBehaviour>(true);

                    for (int j = 0; j < scripts.Length; j++)
                    {
                        if (scripts[j] == null)
                        {
                            Debug.LogError(path);
                            break;
                        }
                    }

                    PrefabUtility.UnloadPrefabContents(go);

                    if (i % feedbackInterval == 0)
                    {
                        _label.text = $"{i}/{guids.Length}";
                        await Task.Delay(20);
                    }
                }

                _label.text = "task finish";
                _isRunning = false;
            }
        }

        private async void OnClickCheckShaderErrors()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _clearConsoleMethod.Invoke(null, null);

                string[] guids = AssetDatabase.FindAssets("t:material", new[] { _pathField.value });
                int feedbackInterval = Mathf.Max(guids.Length / 10, 1);
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

                    if (material.shader == null || material.shader.name == "Hidden/InternalErrorShader")
                    {
                        Debug.LogError(path);
                    }

                    if (i % feedbackInterval == 0)
                    {
                        EditorUtility.UnloadUnusedAssetsImmediate();
                        _label.text = $"{i}/{guids.Length}";
                        await Task.Delay(20);
                    }
                }

                _label.text = "task finish";
                _isRunning = false;
            }
        }

        private async void OnClickFindShaderReferences()
        {
            if (!_isRunning && _shaderField.value != null)
            {
                _isRunning = true;
                _clearConsoleMethod.Invoke(null, null);

                Shader shader = _shaderField.value as Shader;
                string shaderName = shader.name;

                string[] guids = AssetDatabase.FindAssets("t:material", new[] { _pathField.value });
                int feedbackInterval = Mathf.Max(guids.Length / 10, 1);
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

                    if (material.shader.name == shaderName)
                    {
                        Debug.Log(path);
                    }

                    if (i % feedbackInterval == 0)
                    {
                        EditorUtility.UnloadUnusedAssetsImmediate();
                        _label.text = $"{i}/{guids.Length}";
                        await Task.Delay(20);
                    }
                }

                _label.text = "task finish";
                _isRunning = false;
            }
        }

        private async void OnClickFindTextureReferences()
        {
            if (!_isRunning && _textureField.value != null)
            {
                _isRunning = true;
                _clearConsoleMethod.Invoke(null, null);

                Texture texture = _textureField.value as Texture;

                string[] guids = AssetDatabase.FindAssets("t:material", new[] { _pathField.value });
                int feedbackInterval = Mathf.Max(guids.Length / 10, 1);
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

                    int[] nameIds = material.GetTexturePropertyNameIDs();
                    for (int j = 0; j < nameIds.Length; j++)
                    {
                        Texture currentTexture = material.GetTexture(nameIds[j]);
                        if (currentTexture == texture)
                        {
                            Debug.Log(path);
                            break;
                        }
                    }

                    if (i % feedbackInterval == 0)
                    {
                        EditorUtility.UnloadUnusedAssetsImmediate();
                        _label.text = $"{i}/{guids.Length}";
                        await Task.Delay(20);
                    }
                }

                _label.text = "task finish";
                _isRunning = false;
            }
        }

        private async void OnClickFindLayerUsage()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _clearConsoleMethod.Invoke(null, null);

                int targetLayer = _layerField.value;
                string[] guids = AssetDatabase.FindAssets("t:prefab", new[] { _pathField.value });
                int feedbackInterval = Mathf.Max(guids.Length / 10, 1);
                Stack<Transform> stack = new Stack<Transform>();
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    GameObject go = PrefabUtility.LoadPrefabContents(path);
                    stack.Clear();
                    stack.Push(go.transform);

                    while (stack.Count > 0)
                    {
                        Transform target = stack.Pop();
                        if (target.gameObject.layer == targetLayer)
                        {
                            Debug.Log(path);
                            break;
                        }

                        for (int j = 0; j < target.childCount; j++)
                        {
                            stack.Push(target.GetChild(j));
                        }
                    }

                    PrefabUtility.UnloadPrefabContents(go);

                    if (i % feedbackInterval == 0)
                    {
                        _label.text = $"{i}/{guids.Length}";
                        await Task.Delay(20);
                    }
                }

                _label.text = "task finish";
                _isRunning = false;
            }
        }
    }
}