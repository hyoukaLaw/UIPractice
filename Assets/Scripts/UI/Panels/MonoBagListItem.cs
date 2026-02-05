using System;
using TMPro;
using UIModule.Data.Models;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class MonoBagListItem : MonoBehaviour, IBagListItem
    {
        [SerializeField]
        private Image _iconImage;

        [SerializeField]
        private GameObject _selectedBackground;

        [SerializeField]
        private Button _button;
        
        [SerializeField]
        private TextMeshProUGUI _itemNameText;

        private int _index;

        private void Awake()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
        }

        private void OnEnable()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(OnClick);
            }
        }

        private void OnDisable()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClick);
            }
        }

        public void SetData(SerializableBagItem data, int index)
        {
            _index = index;
            if (_iconImage != null)
            {
                _iconImage.sprite = data.GetIcon();
            }
            if (_itemNameText != null)
            {
                _itemNameText.text = data.GetName();
            }
        }

        public void SetSelected(bool selected)
        {
            if (_selectedBackground != null)
            {
                _selectedBackground.SetActive(selected);
            }
        }

        public int GetIndex()
        {
            return _index;
        }

        public event Action<int> OnClickEvent;

        private void OnClick()
        {
            OnClickEvent?.Invoke(_index);
        }
    }
}
