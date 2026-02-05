using UIModule.Data;
using UIModule.Core;

namespace UIModule.Adapters
{
    public static class UIPanelFactory
    {
        public static MonoPanel Create(UIPanelType type)
        {
            var config = UIPanelConfigRegistry.GetConfig(type);
            if (config == null)
            {
                Log.LogError($"No config found for panel type: {type}");
                return null;
            }

            var prefab = UIAssetLoader.Load<UnityEngine.GameObject>(config.PrefabPath);
            if (prefab == null)
            {
                return null;
            }

            if (UICanvasManager.Instance == null)
            {
                Log.LogError("UICanvasManager instance not found. Please ensure UICanvasManager is in scene.");
                return null;
            }

            var layerTransform = UICanvasManager.Instance.GetLayerTransform(config.Layer);
            if (layerTransform == null)
            {
                Log.LogError($"Canvas for layer {config.Layer} not found.");
                return null;
            }

            var instance = UIAssetLoader.Instantiate(prefab, layerTransform);
            if (instance == null)
            {
                return null;
            }

            var monoPanel = instance.GetComponent<MonoPanel>();
            if (monoPanel == null)
            {
                Log.LogError($"Prefab {config.PrefabPath} does not have a MonoPanel component");
                UIAssetLoader.Destroy(instance);
                return null;
            }

            return monoPanel;
        }
    }
}
