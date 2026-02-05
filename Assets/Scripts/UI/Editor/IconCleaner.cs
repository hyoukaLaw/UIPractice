using UnityEditor;
using UnityEngine;
using System.IO;

namespace UIModule.Editor
{
    public class IconCleaner : EditorWindow
    {
        private string _iconPath = "Assets/Resources/Image/BagIcons";

        [MenuItem("Tools/Bag Icon Cleaner")]
        public static void ShowWindow()
        {
            GetWindow<IconCleaner>("Bag Icon Cleaner");
        }

        private void OnGUI()
        {
            GUILayout.Label("Delete Icons (keep only index % 5 == 0)", EditorStyles.boldLabel);
            GUILayout.Space(10);

            _iconPath = EditorGUILayout.TextField("Icon Path", _iconPath);

            GUILayout.Space(20);

            if (GUILayout.Button("Delete Icons", GUILayout.Height(40)))
            {
                DeleteIcons();
            }
        }

        private void DeleteIcons()
        {
            if (!Directory.Exists(_iconPath))
            {
                Debug.LogError($"Directory not found: {_iconPath}");
                return;
            }

            string[] files = Directory.GetFiles(_iconPath, "icon_*.png");
            int totalFiles = files.Length;
            int deletedCount = 0;

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.StartsWith("icon_") && fileName.Length > 5)
                {
                    string numberStr = fileName.Substring(5);
                    if (int.TryParse(numberStr, out int index))
                    {
                        if (index % 5 != 0)
                        {
                            File.Delete(file);
                            deletedCount++;
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"Deleted {deletedCount} icons, kept {totalFiles - deletedCount} icons");
        }
    }
}
