using UIModule.Core;
using UIModule.Data;
using UIModule.Data.Models;
using UIModule.Interfaces;

namespace UIModule.Panels
{
    public class ModalPanel : BaseUIPanel
    {
        private IModalView _modalView;
        private string _title;
        private string _message;
        private System.Action _onConfirm;
        private System.Action _onCancel;

        public ModalPanel(IModalView modalView, BasePanelModel model)
        {
            PanelType = UIPanelType.ModalConfirm;
            IsModal = true;
            _modalView = modalView;
            ParseArgs(model.Args);
        }

        public override void OnEnter(params object[] args)
        {
            Log.LogInfo($"ModalPanel OnEnter: Title={_title}");
            if (_modalView != null)
            {
                _modalView.SetTitle(_title);
                _modalView.SetMessage(_message);
                _modalView.ShowView();
            }
            RegisterCallback();
        }

        public override void OnExit()
        {
            Log.LogInfo($"ModalPanel OnExit");
            UnregisterCallback();
            
            if (_modalView != null)
            {
                _modalView.HideView();
            }
        }

        public override void OnPause()
        {
            Log.LogInfo($"ModalPanel OnPause");
        }

        public override void OnResume()
        {
            Log.LogInfo($"ModalPanel OnResume");
        }

        private void ParseArgs(object[] args)
        {
            _title = "提示";
            _message = "确定要执行此操作吗？";
            _onConfirm = null;
            _onCancel = null;

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

            if (args.Length > 2 && args[2] is System.Action onConfirm)
            {
                _onConfirm = onConfirm;
            }

            if (args.Length > 3 && args[3] is System.Action onCancel)
            {
                _onCancel = onCancel;
            }
        }

        public void RegisterCallback()
        {
            _modalView.OnConfirmClick += OnConfirmClick;
            _modalView.OnCancelClick += OnCancelClick;
        }

        public void UnregisterCallback()
        {
            _modalView.OnConfirmClick -= OnConfirmClick;
            _modalView.OnCancelClick -= OnCancelClick;
        }

        private void OnConfirmClick()
        {
            Log.LogInfo($"ModalPanel OnConfirmClick");
            _onConfirm?.Invoke();
            UIManager.Instance.HidePanel(UIPanelType.ModalConfirm);
        }

        private void OnCancelClick()
        {
            Log.LogInfo($"ModalPanel OnCancelClick");
            _onCancel?.Invoke();
            UIManager.Instance.HidePanel(UIPanelType.ModalConfirm);
        }
    }
}
