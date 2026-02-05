using UnityEngine;

namespace UIModule.Adapters
{
    public class UICanvasManager : MonoBehaviour
    {
        public static UICanvasManager Instance { get; private set; }

        [SerializeField]
        private Canvas normalCanvas;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Multiple UICanvasManager instances detected. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        public Transform GetLayerTransform(Data.UILayer layer)
        {
            return normalCanvas?.transform;
        }

        private void OnValidate()
        {
            if (normalCanvas == null)
            {
                Debug.LogWarning("UICanvasManager: Normal Canvas is not assigned.");
            }
        }
    }
}
