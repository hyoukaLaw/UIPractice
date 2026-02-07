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
            RegisterRedDotCallback();
        }

        public override void OnExit()
        {
            Log.LogInfo($"MainPanel OnExit");
            _view.OnCharacterClick -= OpenCharacterPanel;
            _view.OnBagClick -= OpenBagPanel;
            UnregisterRedDotCallback();
        }

        public override void OnPause()
        {
            Log.LogInfo($"MainPanel OnPause");
        }

        public override void OnResume()
        {
            Log.LogInfo($"MainPanel OnResume");
            RedDotManager.Singleton.MarkRedDotUnitDirty(RedDotUnit.MAIN_UI_CHARACTER_NEW);
        }

        private void RegisterRedDotCallback()
        {
            RedDotManager.Singleton.BindRedDotNameAndReplayCurrent(RedDotNames.MAIN_UI_CHARACTER, RefreshCharacterRedDot);
        }

        private void UnregisterRedDotCallback()
        {
            RedDotManager.Singleton.UnbindRedDotName(RedDotNames.MAIN_UI_CHARACTER, RefreshCharacterRedDot);
        }

        private void RefreshCharacterRedDot(string redDotName, int result, RedDotType redDotType)
        {
            if (redDotName != RedDotNames.MAIN_UI_CHARACTER)
            {
                return;
            }
            _view.SetCharacterRedDot(result > 0);
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
