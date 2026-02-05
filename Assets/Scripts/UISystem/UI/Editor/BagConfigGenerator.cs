using UnityEditor;
using UnityEngine;
using UIModule.Data.Models;

namespace UIModule.Editor
{
    public class BagConfigGenerator : EditorWindow
    {
        private string _configOutputPath = "Assets/Resources/Config/BagConfig.asset";
        private string _iconPath = "Image/BagIcons";

        [MenuItem("Tools/Bag Config Generator")]
        public static void ShowWindow()
        {
            GetWindow<BagConfigGenerator>("Bag Config Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Generate BagConfig with 3000 Items", EditorStyles.boldLabel);
            GUILayout.Space(10);

            _configOutputPath = EditorGUILayout.TextField("Config Output Path", _configOutputPath);
            _iconPath = EditorGUILayout.TextField("Icon Resource Path", _iconPath);

            GUILayout.Space(20);

            if (GUILayout.Button("Generate BagConfig", GUILayout.Height(40)))
            {
                GenerateBagConfig();
            }
        }

        private void GenerateBagConfig()
        {
            BagConfig bagConfig = ScriptableObject.CreateInstance<BagConfig>();
            
            var items = new SerializableBagItem[3000];
            int loadedCount = 0;
            
            for (int i = 0; i < 3000; i++)
            {
                items[i] = new SerializableBagItem();
                items[i].SetId(i.ToString());
                items[i].SetName($"物品{i}");
                items[i].SetCount(1);

                string iconPath = $"{_iconPath}/icon_{i}";
                Sprite icon = Resources.Load<Sprite>(iconPath);
                items[i].SetIcon(icon);

                if (icon != null)
                {
                    loadedCount++;
                }
                else
                {
                    Debug.LogWarning($"Failed to load icon: {iconPath}");
                }
            }
            
            bagConfig.SetItems(new System.Collections.Generic.List<SerializableBagItem>(items));

            string directory = System.IO.Path.GetDirectoryName(_configOutputPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            AssetDatabase.CreateAsset(bagConfig, _configOutputPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Generated BagConfig with 3000 items, {loadedCount} icons loaded");
        }
    }
}
