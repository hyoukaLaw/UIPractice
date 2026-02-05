using System;
using TMPro;
using UIModule.Adapters;
using UIModule.Core;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class MonoModalPanel : MonoPanel, IModalView
    {
        [SerializeField]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        private TextMeshProUGUI _messageText;

        [SerializeField]
        private Button _confirmButton;

        [SerializeField]
        private Button _cancelButton;

        private void Awake()
        {
            if (_confirmButton == null)
            {
                _confirmButton = GetComponentInChildren<Button>();
            }
        }

        private void OnEnable()
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.AddListener(OnConfirmButtonClick);
            }
            if (_cancelButton != null)
            {
                _cancelButton.onClick.AddListener(OnCancelButtonClick);
            }
        }

        private void OnDisable()
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.RemoveListener(OnConfirmButtonClick);
            }
            if (_cancelButton != null)
            {
                _cancelButton.onClick.RemoveListener(OnCancelButtonClick);
            }
        }

        public void SetTitle(string title)
        {
            if (_titleText != null)
            {
                _titleText.text = title;
            }
        }

        public void SetMessage(string message)
        {
            if (_messageText != null)
            {
                _messageText.text = message;
            }
        }

        public void ShowView()
        {
            gameObject.SetActive(true);
        }

        public void HideView()
        {
            gameObject.SetActive(false);
        }

        public event Action OnConfirmClick;
        public event Action OnCancelClick;

        private void OnConfirmButtonClick()
        {
            OnConfirmClick?.Invoke();
        }

        private void OnCancelButtonClick()
        {
            OnCancelClick?.Invoke();
        }
    }
}
