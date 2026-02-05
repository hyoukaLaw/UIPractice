using System;

namespace UIModule.Adapters
{
    public static class UIAssetLoader
    {
        public static T Load<T>(string path) where T : class
        {
            var asset = UnityEngine.Resources.Load<UnityEngine.Object>(path);
            if (asset == null)
            {
                UnityEngine.Debug.LogError($"Failed to load asset at path: {path}");
                return null;
            }

            if (asset is T typedAsset)
            {
                return typedAsset;
            }

            return null;
        }

        public static T Instantiate<T>(T original) where T : UnityEngine.Object
        {
            if (original == null)
            {
                UnityEngine.Debug.LogError("Cannot instantiate null object");
                return null;
            }

            return UnityEngine.Object.Instantiate(original);
        }

        public static T Instantiate<T>(T original, UnityEngine.Transform parent) where T : UnityEngine.Object
        {
            if (original == null)
            {
                UnityEngine.Debug.LogError("Cannot instantiate null object");
                return null;
            }

            return UnityEngine.Object.Instantiate(original, parent);
        }

        public static void Destroy(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                UnityEngine.Object.Destroy(obj);
            }
        }
    }
}
