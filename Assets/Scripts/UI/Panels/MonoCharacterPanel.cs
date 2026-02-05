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
        private GameObject _characterListContent;
        

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnCloseButtonClick);
            _storyPanelButton.onClick.AddListener(OnStoryButtonClick);
        }
        
        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
            _storyPanelButton.onClick.RemoveListener(OnStoryButtonClick);
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
        
        public void OnCloseButtonClick()
        {
            OnCloseClick?.Invoke();
        }
        
        public void OnStoryButtonClick()
        {
            OnStoryPanelClick?.Invoke();
        }

        public class CharacterListItemUI
        {
            public Image CharacterAvatar;
            public int Index;
        }
    }
}
