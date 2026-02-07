using System;
using UIModule.Adapters;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class MonoBagPanel : MonoPanel, IBagView
    {
        [SerializeField]
        private Button _closeButton;
        
        private void OnEnable()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(OnCloseButtonClick);
            }
        }

        private void OnDisable()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(OnCloseButtonClick);
            }
        }
        public void ShowView()
        {
            gameObject.SetActive(true);
        }

        public void HideView()
        {
            gameObject.SetActive(false);
        }

        public event Action OnCloseClick;

        private void OnCloseButtonClick()
        {
            OnCloseClick?.Invoke();
        }
    }
}
