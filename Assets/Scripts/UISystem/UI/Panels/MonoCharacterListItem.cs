using System;
using TMPro;
using UIModule.Data.Models;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class MonoCharacterListItem : MonoBehaviour, ICharacterListItem
    {
        [SerializeField]
        private Image _avatarImage;

        [SerializeField]
        private GameObject _selectedBackground;

        [SerializeField]
        private Button _button;
        
        [SerializeField]
        private RedDotWidget _redDotWidget;

        private int _index;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void SetData(SerializableCharacterData data, int index)
        {
            _index = index;
            if (_avatarImage != null)
            {
                _avatarImage.sprite = data.GetAvatarSprite();
            }
        }

        public void SetSelected(bool selected)
        {
            if (_selectedBackground != null)
            {
                _selectedBackground.SetActive(selected);
            }
        }

        public void SetRedDot(bool show)
        {
            _redDotWidget.gameObject.SetActive(show);
        }

        public event Action<int> OnClickEvent;

        private void OnClick()
        {
            OnClickEvent?.Invoke(_index);
        }

        public int GetIndex()
        {
            return _index;
        }
    }
}
