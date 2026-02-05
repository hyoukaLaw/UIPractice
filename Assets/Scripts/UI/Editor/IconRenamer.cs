using UnityEditor;
using UnityEngine;
using System.IO;

namespace UIModule.Editor
{
    public class IconRenamer : EditorWindow
    {
        private string _iconPath = "Assets/Resources/Image/BagIcons";
        private string _tempPath = "Assets/Resources/Image/BagIcons_Temp";

        [MenuItem("Tools/Bag Icon Renamer")]
        public static void ShowWindow()
        {
            GetWindow<IconRenamer>("Bag Icon Renamer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Rename Icons (keep % 5 == 0, renumber sequentially)", EditorStyles.boldLabel);
            GUILayout.Space(10);

            _iconPath = EditorGUILayout.TextField("Icon Path", _iconPath);
            _tempPath = EditorGUILayout.TextField("Temp Path", _tempPath);

            GUILayout.Space(20);

            if (GUILayout.Button("Rename Icons", GUILayout.Height(40)))
            {
                RenameIcons();
            }
        }

        private void RenameIcons()
        {
            if (!Directory.Exists(_iconPath))
            {
                Debug.LogError($"Directory not found: {_iconPath}");
                return;
            }

            string[] allFiles = Directory.GetFiles(_iconPath, "icon_*.png");
            
            var filesToKeep = new System.Collections.Generic.List<string>();
            
            foreach (string file in allFiles)
            {
                int? index = GetIconIndex(file);
                if (index.HasValue && index.Value % 5 == 0)
                {
                    filesToKeep.Add(file);
                }
            }
            
            filesToKeep.Sort((a, b) => GetIconIndex(a).Value.CompareTo(GetIconIndex(b).Value));
            
            Directory.CreateDirectory(_tempPath);
            int renamedCount = 0;

            for (int i =0; i < filesToKeep.Count; i++)
            {
                string newPath = Path.Combine(_tempPath, $"icon_{i}.png");
                File.Copy(filesToKeep[i], newPath);
                renamedCount++;
            }

            Directory.Delete(_iconPath, true);
            Directory.Move(_tempPath, _iconPath);

            Debug.Log($"Renamed {renamedCount} icons sequentially");
        }

        private int? GetIconIndex(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName.StartsWith("icon_") && fileName.Length > 5)
            {
                string numberStr = fileName.Substring(5);
                if (int.TryParse(numberStr, out int index))
                {
                    return index;
                }
            }
            return null;
        }
    
        
    }
}
