using System;
using System.Collections.Generic;
using TMPro;
using UIModule.Adapters;
using UIModule.Core;
using UIModule.Data;
using UIModule.Data.Models;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class MonoCharacterPanel : MonoPanel, ICharacterView
    {
        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private TextMeshProUGUI _characterNameText;

        [SerializeField]
        private TextMeshProUGUI _levelText;

        [SerializeField]
        private Button _storyPanelButton;
        
        [SerializeField]
        private RedDotWidget _storyPanelRedDotWidget;
        
        [SerializeField]
        private RedDotWidget _cgPanelRedDotWidget;
        
        [SerializeField]
        private Button _cgPanelButton;

        [SerializeField] 
        private GameObject _characterListContent;
        

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnCloseButtonClick);
            _storyPanelButton.onClick.AddListener(OnStoryButtonClick);
            _cgPanelButton.onClick.AddListener(OnCgButtonClick);
        }
        
        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
            _storyPanelButton.onClick.RemoveListener(OnStoryButtonClick);
            _cgPanelButton.onClick.RemoveListener(OnCgButtonClick);
        }

        public void SetCharacterName(string characterName)
        {
             _characterNameText.text = characterName;
        }

        public void SetLevel(int level)
        {
            _levelText.text = $"Lv.{level}";
        }
        
        public void SetCharacterList(List<GameObject> characterListItems)
        {
            foreach (var characterListItem in characterListItems)
            {
                characterListItem.transform.SetParent(_characterListContent.transform, false);
                
            }
        }

        public void SetStoryRedDot(bool show)
        {
            _storyPanelRedDotWidget.gameObject.SetActive(show);
        }
        
        public void SetCgRedDot(bool show)
        {
            _cgPanelRedDotWidget.gameObject.SetActive(show);
        }

        public void SetSelectedCharacter(int index)
        {
        }

        public void ShowView()
        {
             
        }

        public void HideView()
        {
             
        }

        public event Action OnCloseClick;
        public event Action OnStoryPanelClick;
        public event Action OnCgPanelClick;
        
        public void OnCloseButtonClick()
        {
            OnCloseClick?.Invoke();
        }
        
        public void OnStoryButtonClick()
        {
            OnStoryPanelClick?.Invoke();
        }
        
        public void OnCgButtonClick()
        {
            OnCgPanelClick?.Invoke();
        }
    }
}
