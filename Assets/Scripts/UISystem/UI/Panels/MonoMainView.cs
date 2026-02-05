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
            RedDotManager.Singleton.BindRedDotName(RedDotNames.MAIN_UI_CHARACTER, OnRedDotRefresh);
            (int result, RedDotType redDotType) redDotNameResult;
            redDotNameResult = RedDotManager.Singleton.GetRedDotNameResult(RedDotNames.MAIN_UI_CHARACTER);
            OnRedDotRefresh(RedDotNames.MAIN_UI_CHARACTER, redDotNameResult.result, redDotNameResult.redDotType);
        }

        private void OnDisable()
        {
            _characterPanelButton.onClick.RemoveListener(OnCharacterButtonClick);
            _bagPanelButton.onClick.RemoveListener(OnBagButtonClick);
            RedDotManager.Singleton.UnbindRedDotName(RedDotNames.MAIN_UI_CHARACTER, OnRedDotRefresh);
        }

        private void OnCharacterButtonClick()
        {
            OnCharacterClick?.Invoke();
        }
        
        private void OnBagButtonClick()
        {
            OnBagClick?.Invoke();
        }

        private void OnRedDotRefresh(string redDotName, int result, RedDotType redDotType)
        {
            if (redDotName == RedDotNames.MAIN_UI_CHARACTER)
            {
                _redDotWidget.gameObject.SetActive(result > 0);
            }
        }
    }
}
