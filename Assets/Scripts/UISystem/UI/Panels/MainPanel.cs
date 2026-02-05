using UIModule.Data;
using UIModule.Core;
using UIModule.Interfaces;

namespace UIModule.Panels
{
    public class MainPanel : BaseUIPanel
    {
        private IMainView _view;
        public MainPanel(IMainView view)
        {
            PanelType = UIPanelType.Main;
            IsModal = false;
            _view = view;
        }

        public override void OnEnter(params object[] args)
        {
            Log.LogInfo($"MainPanel OnEnter");
            _view.OnCharacterClick += OpenCharacterPanel;
            _view.OnBagClick += OpenBagPanel;
        }

        public override void OnExit()
        {
            Log.LogInfo($"MainPanel OnExit");
            _view.OnCharacterClick -= OpenCharacterPanel;
            _view.OnBagClick -= OpenBagPanel;
        }

        public override void OnPause()
        {
            Log.LogInfo($"MainPanel OnPause");
        }

        public override void OnResume()
        {
            Log.LogInfo($"MainPanel OnResume");
        }

        private void OpenCharacterPanel()
        {
            UIManager.Instance.ShowPanel(UIPanelType.Character);
        }
        
        private void OpenBagPanel()
        {
            UIManager.Instance.ShowPanel(UIPanelType.Bag);
        }
    }
}
