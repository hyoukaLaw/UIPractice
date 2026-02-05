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

        private void OnEnable()
        {
            _characterPanelButton.onClick.AddListener(OnCharacterButtonClick);
        }

        private void OnDisable()
        {
            _characterPanelButton.onClick.RemoveListener(OnCharacterButtonClick);
        }

        private void OnCharacterButtonClick()
        {
            OnCharacterClick?.Invoke();
        }
    }
}
