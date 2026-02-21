using System.Collections.Generic;
using UIModule.Core;
using UIModule.Data;
using UIModule.Data.Models;
using UIModule.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIModule.Panels
{
    public class BagPanel : BaseUIPanel
    {
        private const float _defaultItemSize = 100f;
        private const int _defaultVerticalColumnCount = 7;
        private const int _defaultHorizontalRowCount = 3;

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
            ScrollType scrollType = args?.Length > 1 && args[1] is ScrollType type ? type : ScrollType.Vertical;
            int primaryCount = ResolvePrimaryCount(args, scrollType);
            BagConfig bagConfig = Resources.Load<BagConfig>(configPath);
            _model.SetBagConfig(bagConfig);
            var view = _bagView as MonoBagPanel;
            if (view != null)
            {
                ConfigureScrollRectDirection(view, scrollType);
                BagItemPool pool = view.GetComponentInChildren<BagItemPool>();
                if (pool != null)
                {
                    pool.Initialize(bagConfig);
                }
                InfiniteScrollRect scrollRect = view.GetComponentInChildren<InfiniteScrollRect>();
                if (scrollRect != null && bagConfig != null)
                {
                    scrollRect.Initialize(bagConfig.GetItems().Count, _defaultItemSize, _defaultItemSize, primaryCount, scrollType);
                }
            }
            RegisterCallback();
        }

        private int ResolvePrimaryCount(object[] args, ScrollType scrollType)
        {
            if (args?.Length > 2 && args[2] is int count)
            {
                return count;
            }
            if (scrollType == ScrollType.Horizontal)
            {
                return _defaultHorizontalRowCount;
            }
            return _defaultVerticalColumnCount;
        }

        private void ConfigureScrollRectDirection(MonoBagPanel view, ScrollType scrollType)
        {
            var unityScrollRect = view.GetComponentInChildren<ScrollRect>();
            if (unityScrollRect == null)
            {
                return;
            }
            if (scrollType == ScrollType.Horizontal)
            {
                unityScrollRect.horizontal = true;
                unityScrollRect.vertical = false;
                unityScrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
                unityScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
                return;
            }
            unityScrollRect.horizontal = false;
            unityScrollRect.vertical = true;
            unityScrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            unityScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
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
