using System;
using System.Collections.Generic;
using TMPro;
using UIModule.Adapters;
using UIModule.Core;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class MonoBagPanel : MonoPanel, IBagView
    {
        [SerializeField]
        private Button _closeButton;
        [SerializeField]
        private InfiniteScrollRect _infiniteScrollRect;
        
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
        

        public void SetSelectedItemIndex(int index)
        {
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
        public event Action<int> OnItemClick;

        private void OnCloseButtonClick()
        {
            OnCloseClick?.Invoke();
        }
    }
}
