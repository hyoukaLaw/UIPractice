using System;
using UIModule.Adapters;
using UIModule.Core;
using UIModule.Data;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class MonoCharacterStoryPanel : MonoPanel, ICharacterStoryView
    {
        [SerializeField]
        public Button _closeButton;
        public event Action OnCloseClick = delegate { };
        
        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnCloseButtonClick);
        }
        
        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
        }
        
        private void OnCloseButtonClick()
        {
            OnCloseClick?.Invoke();
        }
    }
}
