using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GUIGUI17F
{
    public class MeshFileExporter
    {
        [MenuItem("Assets/GUIGUI17F/Export Mesh File")]
        public static void ExportMeshFile()
        {
            var selected = Selection.GetFiltered<Mesh>(SelectionMode.Assets);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < selected.Length; i++)
            {
                for (int j = 0; j < selected[i].vertices.Length; j++)
                {
                    builder.AppendLine($"v {selected[i].vertices[j].x} {selected[i].vertices[j].y} {selected[i].vertices[j].z}");
                }
                for (int j = 0; j < selected[i].uv.Length; j++)
                {
                    builder.AppendLine($"vt {selected[i].uv[j].x} {selected[i].uv[j].y}");
                }
                for (int j = 0; j < selected[i].triangles.Length; j += 3)
                {
                    builder.AppendLine($"f {selected[i].triangles[j] + 1} {selected[i].triangles[j + 1] + 1} {selected[i].triangles[j + 2] + 1}");
                }
                var path = AssetDatabase.GetAssetPath(selected[i]);
                var fileName = Path.GetFileNameWithoutExtension(path);
                var directory = Path.GetDirectoryName(path);
                var savePath = Path.Combine(directory, $"{fileName}.obj");
                File.WriteAllText(savePath, builder.ToString());
                builder.Clear();
            }
            AssetDatabase.Refresh();
        }
    }
}