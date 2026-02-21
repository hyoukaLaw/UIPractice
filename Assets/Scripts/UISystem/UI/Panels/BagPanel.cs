using System.Collections.Generic;
using UIModule.Core;
using UIModule.Data;
using UIModule.Data.Models;
using UIModule.Interfaces;
using UnityEngine;

namespace UIModule.Panels
{
    public class BagPanel : BaseUIPanel
    {
        private IBagView _bagView;
        private BagPanelModel _model;

        public BagPanel(IBagView bagView, BasePanelModel model)
        {
            PanelType = UIPanelType.Bag;
            IsModal = false;
            _bagView = bagView;
            _model = (BagPanelModel)model;
        }

        public override void OnEnter(params object[] args)
        {
            string configPath = args?.Length > 0 ? args[0]?.ToString() : "Config/BagConfig";
            BagConfig bagConfig = Resources.Load<BagConfig>(configPath);
            _model.SetBagConfig(bagConfig);
            var view = _bagView as MonoBagPanel;
            if (view != null)
            {
                BagItemPool pool = view.GetComponentInChildren<BagItemPool>();
                if (pool != null)
                {
                    pool.Initialize(bagConfig);
                }
                InfiniteScrollRect scrollRect = view.GetComponentInChildren<InfiniteScrollRect>();
                if (scrollRect != null && bagConfig != null)
                {
                    scrollRect.Initialize(bagConfig.GetItems().Count, 100f, 100f, 7, ScrollType.Vertical);
                }
            }
            RegisterCallback();
        }

        public override void OnExit()
        {
            Log.LogInfo("BagPanel OnExit");
            UnregisterCallback();
        }

        public override void OnPause()
        {
            Log.LogInfo("BagPanel OnPause");
        }

        public override void OnResume()
        {
            Log.LogInfo("BagPanel OnResume");
        }

        public void RegisterCallback()
        {
            _bagView.OnCloseClick += CloseCurrent;
        }

        public void UnregisterCallback()
        {
            _bagView.OnCloseClick -= CloseCurrent;
        }

        private void CloseCurrent()
        {
            UIManager.Instance.HidePanel(UIPanelType.Bag);
        }
    }
}
