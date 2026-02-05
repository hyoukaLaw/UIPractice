using System;
using TMPro;
using UIModule.Adapters;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class MonoAlertPanel : MonoPanel, IModalView
    {
        [SerializeField]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        private TextMeshProUGUI _messageText;

        [SerializeField]
        private Button _okButton;

        private void OnEnable()
        {
            if (_okButton != null)
            {
                _okButton.onClick.AddListener(OnOkButtonClick);
            }
        }

        private void OnDisable()
        {
            if (_okButton != null)
            {
                _okButton.onClick.RemoveListener(OnOkButtonClick);
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

        private void OnOkButtonClick()
        {
            OnConfirmClick?.Invoke();
        }
    }
}
