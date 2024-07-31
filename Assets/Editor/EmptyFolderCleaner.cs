using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GUIGUI17F
{
    //an editor script to clean empty folders automatically when project open
    public class EmptyFolderCleaner
    {
        [InitializeOnLoadMethod]
        private static void CleanEmptyFolders()
        {
            var deleted = new List<string>();
            var folders = new List<string>(Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories));
            //sort in reverse order, ensure subfolders are checked first
            folders.Sort((a, b) => b.Length - a.Length);
            foreach (var folder in folders)
            {
                var files = Directory.GetFiles(folder);
                var subFolders = Directory.GetDirectories(folder);
                if (files.Length == 0 && subFolders.Length == 0)
                {
                    Directory.Delete(folder);
                    deleted.Add(folder);
                    var metaPath = folder + ".meta";
                    if (File.Exists(metaPath))
                    {
                        File.Delete(metaPath);
                    }
                }
            }
            foreach (var folder in deleted)
            {
                Debug.Log($"deleted empty folder {folder}");
            }
        }
    }
}