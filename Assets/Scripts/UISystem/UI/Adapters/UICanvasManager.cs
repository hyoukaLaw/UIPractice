using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Adapters
{
    public class UICanvasManager : MonoBehaviour
    {
        public static UICanvasManager Instance { get; private set; }

        [SerializeField]
        private Canvas _backgroundCanvas;

        [SerializeField]
        private Canvas _normalCanvas;

        [SerializeField]
        private Canvas _modalCanvas;

        [SerializeField]
        private Image _modalBackgroundImage;

        [SerializeField]
        private Canvas _popupCanvas;

        [SerializeField]
        private Canvas _topCanvas;

        private int _modalCount = 0;

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
            return layer switch
            {
                Data.UILayer.Background => _backgroundCanvas?.transform,
                Data.UILayer.Normal => _normalCanvas?.transform,
                Data.UILayer.Modal => _modalCanvas?.transform,
                Data.UILayer.Popup => _popupCanvas?.transform,
                Data.UILayer.Top => _topCanvas?.transform,
                _ => _normalCanvas?.transform
            };
        }

        public void ShowModalBackground()
        {
            if (_modalCount == 0)
            {
                if (_modalBackgroundImage != null)
                {
                    _modalBackgroundImage.gameObject.SetActive(true);
                }
            }
            _modalCount++;
        }

        public void HideModalBackground()
        {
            _modalCount--;
            if (_modalCount <= 0)
            {
                _modalCount = 0;
                if (_modalBackgroundImage != null)
                {
                    _modalBackgroundImage.gameObject.SetActive(false);
                }
            }
        }

        private void OnValidate()
        {
            if (_normalCanvas == null)
            {
                Debug.LogWarning("UICanvasManager: Normal Canvas is not assigned.");
            }
            if (_backgroundCanvas == null)
            {
                Debug.LogWarning("UICanvasManager: Background Canvas is not assigned.");
            }
            if (_modalCanvas == null)
            {
                Debug.LogWarning("UICanvasManager: Modal Canvas is not assigned.");
            }
            if (_modalBackgroundImage == null)
            {
                Debug.LogWarning("UICanvasManager: Modal Background Image is not assigned.");
            }
            if (_popupCanvas == null)
            {
                Debug.LogWarning("UICanvasManager: Popup Canvas is not assigned.");
            }
            if (_topCanvas == null)
            {
                Debug.LogWarning("UICanvasManager: Top Canvas is not assigned.");
            }
        }
    }
}
