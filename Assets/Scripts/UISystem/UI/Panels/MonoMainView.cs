using System;
using UIModule.Adapters;
using UIModule.Core;
using UIModule.Data;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class MonoMainView : MonoPanel, IMainView
    {
        public event Action OnCharacterClick;
        public event Action OnBagClick;
        [SerializeField]
        private Button _characterPanelButton;
        [SerializeField]
        private Button _bagPanelButton;
        [SerializeField]
        private RedDotWidget _redDotWidget;

        private void OnEnable()
        {
            _characterPanelButton.onClick.AddListener(OnCharacterButtonClick);
            _bagPanelButton.onClick.AddListener(OnBagButtonClick);
        }

        private void OnDisable()
        {
            _characterPanelButton.onClick.RemoveListener(OnCharacterButtonClick);
            _bagPanelButton.onClick.RemoveListener(OnBagButtonClick);
        }

        private void OnCharacterButtonClick()
        {
            OnCharacterClick?.Invoke();
        }
        
        private void OnBagButtonClick()
        {
            OnBagClick?.Invoke();
        }

        public void SetCharacterRedDot(bool show)
        {
            _redDotWidget.gameObject.SetActive(show);
        }
    }
}
