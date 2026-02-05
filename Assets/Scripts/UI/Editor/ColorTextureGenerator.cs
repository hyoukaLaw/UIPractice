using UnityEditor;
using UnityEngine;

namespace UIModule.Editor
{
    public class ColorTextureGenerator : EditorWindow
    {
        private string _outputPath = "Assets/Resources/Image/BagIcons";
        private int _width = 64;
        private int _height = 64;

        [MenuItem("Tools/Bag Icon Generator")]
        public static void ShowWindow()
        {
            GetWindow<ColorTextureGenerator>("Bag Icon Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Generate Color Gradient Textures", EditorStyles.boldLabel);
            GUILayout.Space(10);

            _outputPath = EditorGUILayout.TextField("Output Path", _outputPath);
            _width = EditorGUILayout.IntField("Width", _width);
            _height = EditorGUILayout.IntField("Height", _height);

            GUILayout.Space(20);

            if (GUILayout.Button("Generate Textures", GUILayout.Height(40)))
            {
                GenerateTextures();
            }
        }

        private void GenerateTextures()
        {
            if (!AssetDatabase.IsValidFolder(_outputPath))
            {
                string parentPath = System.IO.Path.GetDirectoryName(_outputPath);
                string folderName = System.IO.Path.GetFileName(_outputPath);
                if (!AssetDatabase.IsValidFolder(parentPath))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                    AssetDatabase.CreateFolder("Assets/Resources", "Image");
                }
                if (!AssetDatabase.IsValidFolder(_outputPath))
                {
                    AssetDatabase.CreateFolder(parentPath, folderName);
                }
            }

            int count = 0;
            for (int r = 0; r <= 255; r += 8)
            {
                for (int g = 0; g <= 255; g += 8)
                {
                    for (int b = 0; b <= 255; b += 8)
                    {
                        Color color = new Color(r / 255f, g / 255f, b / 255f);
                        Texture2D texture = new Texture2D(_width, _height);
                        Color[] pixels = new Color[_width * _height];
                        for (int i = 0; i < pixels.Length; i++)
                        {
                            pixels[i] = color;
                        }
                        texture.SetPixels(pixels);
                        texture.Apply();

                        byte[] bytes = texture.EncodeToPNG();
                        string filePath = $"{_outputPath}/icon_{count:D4}.png";
                        System.IO.File.WriteAllBytes(filePath, bytes);
                        count++;
                    }
                }
            }

            AssetDatabase.Refresh();
            
            for (int i = 0; i < count; i++)
            {
                string filePath = $"{_outputPath}/icon_{i:D4}.png";
                TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.SaveAndReimport();
                }
            }
            
            Debug.Log($"Generated {count} sprites to {_outputPath}");
        }
    }
}
