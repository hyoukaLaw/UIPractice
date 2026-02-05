using UIModule.Core;
using UIModule.Data;
using UIModule.Data.Models;
using UIModule.Interfaces;

namespace UIModule.Panels
{
    public class AlertPanel : BaseUIPanel
    {
        private IModalView _alertView;
        private string _title;
        private string _message;
        private System.Action _onOk;

        public AlertPanel(IModalView alertView, BasePanelModel model)
        {
            PanelType = UIPanelType.ModalAlert;
            IsModal = true;
            _alertView = alertView;
            ParseArgs(model.Args);
        }

        public override void OnEnter(params object[] args)
        {
            Log.LogInfo($"AlertPanel OnEnter: Title={_title}");
            
            if (_alertView != null)
            {
                _alertView.SetTitle(_title);
                _alertView.SetMessage(_message);
                _alertView.ShowView();
            }
            
            RegisterCallback();
        }

        public override void OnExit()
        {
            Log.LogInfo($"AlertPanel OnExit");
            UnregisterCallback();
            
            if (_alertView != null)
            {
                _alertView.HideView();
            }
        }

        public override void OnPause()
        {
            Log.LogInfo($"AlertPanel OnPause");
        }

        public override void OnResume()
        {
            Log.LogInfo($"AlertPanel OnResume");
        }

        private void ParseArgs(object[] args)
        {
            _title = "警告";
            _message = "发生错误，请重试";
            _onOk = null;

            if (args == null || args.Length == 0)
            {
                return;
            }

            if (args.Length > 0 && args[0] is string title)
            {
                _title = title;
            }

            if (args.Length > 1 && args[1] is string message)
            {
                _message = message;
            }

            if (args.Length > 2 && args[2] is System.Action onOk)
            {
                _onOk = onOk;
            }
        }

        public void RegisterCallback()
        {
            _alertView.OnConfirmClick += OnOkClick;
        }

        public void UnregisterCallback()
        {
            _alertView.OnConfirmClick -= OnOkClick;
        }

        private void OnOkClick()
        {
            Log.LogInfo($"AlertPanel OnOkClick");
            _onOk?.Invoke();
            UIManager.Instance.HidePanel(UIPanelType.ModalAlert);
        }
    }
}
