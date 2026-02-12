using System;
using System.Collections.Generic;
using TMPro;
using UIModule.Data.Models;
using UIModule.Data;
using UIModule.Adapters;

namespace UIModule.Core
{
    public class UIManager
    {
        private struct PanelRuntimeContext
        {
            public UIPanelType PanelType;
            public UIPanelConfig Config;
            public MonoPanel MonoPanel;
            public BasePanelModel PanelModel;
            public BaseUIPanel Panel;
            public UIPanelData PanelData;
        }

        private static readonly Lazy<UIManager> _instance = new(() => new UIManager());

        public static UIManager Instance => _instance.Value;

        private readonly UIStack _stack = new UIStack();

        private readonly Dictionary<UIPanelType, BaseUIPanel> _activePanels = new Dictionary<UIPanelType, BaseUIPanel>();
        private readonly Dictionary<UIPanelType, MonoPanel> _activeMonoPanels = new Dictionary<UIPanelType, MonoPanel>();
        private readonly Dictionary<UIPanelType, BasePanelModel> _activePanelModels = new Dictionary<UIPanelType, BasePanelModel>();
        private TMP_FontAsset _dynamicFontAsset;

        private UIManager()
        {
        }

        public void SetDynamicFontAsset(TMP_FontAsset fontAsset)
        {
            _dynamicFontAsset = fontAsset;
        }

        public void ShowPanel(UIPanelType type, params object[] args)
        {
            PauseTopPanelIfNeeded();
            if (IsPanelActive(type))
            {
                Log.LogWarning($"Panel {type} is already active");
                return;
            }
            if (!TryCreatePanelRuntimeContext(type, out var context))
            {
                return;
            }
            ShowModalBackgroundIfNeeded(context.Config);
            RegisterPanelContext(context);
            context.Panel.OnEnter(args);
        }

        public void HidePanel(UIPanelType type)
        {
            if (!TryGetActivePanelContext(type, out var context))
            {
                return;
            }
            context.Panel.OnExit();
            RemovePanelFromStackIfExists(type);
            UnregisterActivePanel(type);
            UIAssetLoader.Destroy(context.MonoPanel.gameObject);
            HideModalBackgroundIfNeeded(context.Config);
            ResumeTopPanelIfNeeded();
            UnloadDynamicFontAsset();
        }

        public void HideTopPanel()
        {
            if (_stack.Count == 0)
            {
                return;
            }

            var topPanelData = _stack.Peek();
            if (topPanelData != null)
            {
                HidePanel(topPanelData.PanelType);
            }
        }

        private UIPanelData FindPanelInStack(UIPanelType type)
        {
            var tempStack = new Stack<UIPanelData>();
            UIPanelData found = null;

            while (_stack.Count > 0)
            {
                var panel = _stack.Pop();
                if (panel.PanelType == type && found == null)
                {
                    found = panel;
                }
                tempStack.Push(panel);
            }

            while (tempStack.Count > 0)
            {
                _stack.Push(tempStack.Pop());
            }

            return found;
        }

        private void RemovePanelFromStack(UIPanelType type)
        {
            var tempStack = new Stack<UIPanelData>();
            UIPanelData removed = null;

            while (_stack.Count > 0)
            {
                var panel = _stack.Pop();
                if (panel.PanelType == type && removed == null)
                {
                    removed = panel;
                }
                else
                {
                    tempStack.Push(panel);
                }
            }

            while (tempStack.Count > 0)
            {
                _stack.Push(tempStack.Pop());
            }
        }
        
        public BasePanelModel FindPanelModel(UIPanelType type)
        {
            if (_activePanelModels.TryGetValue(type, out var model))
                return model;
            return null;
        }

        private void PauseTopPanelIfNeeded()
        {
            var topPanelData = _stack.Peek();
            if (topPanelData == null)
            {
                return;
            }
            if (_activePanels.TryGetValue(topPanelData.PanelType, out var topPanel))
            {
                topPanel.OnPause();
            }
        }

        private void ResumeTopPanelIfNeeded()
        {
            if (_stack.Count == 0)
            {
                return;
            }
            var topPanelData = _stack.Peek();
            if (topPanelData == null)
            {
                return;
            }
            if (_activePanels.TryGetValue(topPanelData.PanelType, out var topPanel))
            {
                topPanel.OnResume();
            }
        }

        private void UnloadDynamicFontAsset()
        {
            _dynamicFontAsset?.ClearFontAssetData();
        }

        private bool IsPanelActive(UIPanelType type)
        {
            return _activePanels.ContainsKey(type);
        }

        private bool TryCreatePanelRuntimeContext(UIPanelType type, out PanelRuntimeContext context)
        {
            context = default;
            var config = UIPanelConfigRegistry.GetConfig(type);
            var monoPanel = UIPanelFactory.Create(type);
            if (monoPanel == null)
            {
                Log.LogError($"Create MonoPanel failed for type: {type}");
                return false;
            }
            var panelModel = UIPanelModelFactory.Create(type);
            var panel = UIPresenterFactory.Create(type, monoPanel, panelModel);
            if (panel == null)
            {
                Log.LogError($"Create Panel presenter failed for type: {type}");
                return false;
            }
            context.PanelType = type;
            context.Config = config;
            context.MonoPanel = monoPanel;
            context.PanelModel = panelModel;
            context.Panel = panel;
            context.PanelData = new UIPanelData(type, panel.IsModal);
            return true;
        }

        private void RegisterPanelContext(PanelRuntimeContext context)
        {
            _stack.Push(context.PanelData);
            _activePanels.Add(context.PanelType, context.Panel);
            _activeMonoPanels.Add(context.PanelType, context.MonoPanel);
            if (context.PanelModel != null)
            {
                _activePanelModels[context.PanelType] = context.PanelModel;
            }
        }

        private bool TryGetActivePanelContext(UIPanelType type, out PanelRuntimeContext context)
        {
            context = default;
            if (!_activePanels.TryGetValue(type, out var panel))
            {
                Log.LogWarning($"Panel {type} is not active");
                return false;
            }
            if (!_activeMonoPanels.TryGetValue(type, out var monoPanel))
            {
                Log.LogError($"Panel {type} does not have a MonoPanel component");
                return false;
            }
            context.PanelType = type;
            context.Config = UIPanelConfigRegistry.GetConfig(type);
            context.Panel = panel;
            context.MonoPanel = monoPanel;
            return true;
        }

        private void UnregisterActivePanel(UIPanelType type)
        {
            _activePanels.Remove(type);
            _activeMonoPanels.Remove(type);
            _activePanelModels.Remove(type);
        }

        private void RemovePanelFromStackIfExists(UIPanelType type)
        {
            var panelData = FindPanelInStack(type);
            if (panelData != null)
            {
                RemovePanelFromStack(type);
            }
        }

        private void ShowModalBackgroundIfNeeded(UIPanelConfig config)
        {
            if (config?.IsModal == true)
            {
                UICanvasManager.Instance?.ShowModalBackground();
            }
        }

        private void HideModalBackgroundIfNeeded(UIPanelConfig config)
        {
            if (config?.IsModal == true)
            {
                UICanvasManager.Instance?.HideModalBackground();
            }
        }
    }
}
