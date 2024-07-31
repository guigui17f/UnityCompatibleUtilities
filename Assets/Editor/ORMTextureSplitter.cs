using UnityEditor;
using UnityEngine;

namespace GUIGUI17F
{
    public class ORMTextureSplitter : EditorWindow
    {
        private Texture2D _ormTexture;

        [MenuItem("DKS/Split ORM Texture")]
        public static void ShowWindow()
        {
            var window = GetWindow<ORMTextureSplitter>("ORM Texture Splitter");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Choose ORM Texture:", EditorStyles.boldLabel);
            _ormTexture = (Texture2D)EditorGUILayout.ObjectField("ORM Texture", _ormTexture, typeof(Texture2D), false);
            if (GUILayout.Button("Split Texture") && _ormTexture != null)
            {
                SplitTexture(_ormTexture);
            }
        }

        private void SplitTexture(Texture2D sourceTexture)
        {
            string path = AssetDatabase.GetAssetPath(sourceTexture);
            string directory = System.IO.Path.GetDirectoryName(path);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            // Create Occlusion Map
            Texture2D occlusionMap = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGB24, true);
            occlusionMap.SetPixels(ExtractChannel(sourceTexture, 0));
            SaveTextureAsPNG(occlusionMap, $"{directory}/{fileName}_Occlusion.png");

            // Create MetallicSmoothness Map
            Texture2D metallicSmoothnessMap = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, true);
            metallicSmoothnessMap.SetPixels(CreateMetallicSmoothnessMap(sourceTexture));
            SaveTextureAsPNG(metallicSmoothnessMap,  $"{directory}/{fileName}_MetallicSmoothness.png");

            AssetDatabase.Refresh();
        }

        private Color[] ExtractChannel(Texture2D source, int channel)
        {
            Color[] pixels = source.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                float channelValue = pixels[i][channel];
                pixels[i] = new Color(channelValue, channelValue, channelValue);
            }
            return pixels;
        }

        private Color[] CreateMetallicSmoothnessMap(Texture2D source)
        {
            Color[] sourcePixels = source.GetPixels();
            Color[] newPixels = new Color[sourcePixels.Length];
            for (int i = 0; i < sourcePixels.Length; i++)
            {
                float metallic = sourcePixels[i].b; // Metallic from Blue channel
                float smoothness = 1f - sourcePixels[i].g; // Smoothness from inverted Green channel
                newPixels[i] = new Color(metallic, 0, 0, smoothness);
            }
            return newPixels;
        }

        private void SaveTextureAsPNG(Texture2D texture, string path)
        {
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            DestroyImmediate(texture); // Clean up the texture from memory
        }
    }
}