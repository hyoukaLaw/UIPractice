using System;
using UIModule.Adapters;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class MonoMainView : MonoPanel, IMainView
    {
        public event Action OnCharacterClick;
        public event Action OnBagClick;
        public event Action OnBagHorizontalClick;
        [SerializeField]
        private Button _characterPanelButton;
        [SerializeField]
        private Button _bagPanelButton;

        [SerializeField] 
        private Button _bagPanelHorizontalButton;
        [SerializeField]
        private RedDotWidget _redDotWidget;

        private void OnEnable()
        {
            _characterPanelButton.onClick.AddListener(OnCharacterButtonClick);
            _bagPanelButton.onClick.AddListener(OnBagButtonClick);
            _bagPanelHorizontalButton.onClick.AddListener(OnBagButtonHorizontalClick);
        }

        private void OnDisable()
        {
            _characterPanelButton.onClick.RemoveListener(OnCharacterButtonClick);
            _bagPanelButton.onClick.RemoveListener(OnBagButtonClick);
            _bagPanelHorizontalButton.onClick.RemoveListener(OnBagButtonHorizontalClick);
        }

        private void OnCharacterButtonClick()
        {
            OnCharacterClick?.Invoke();
        }
        
        private void OnBagButtonClick()
        {
            OnBagClick?.Invoke();
        }

        private void OnBagButtonHorizontalClick()
        {
            OnBagHorizontalClick?.Invoke();
        }

        public void SetCharacterRedDot(bool show)
        {
            _redDotWidget.gameObject.SetActive(show);
        }
    }
}
