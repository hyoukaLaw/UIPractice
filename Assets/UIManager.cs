using System;
using System.Collections.Generic;
using UIModule.Data.Models;

namespace UIModule.Core
{
    using Data;
    using Adapters;

    public class UIManager
    {
        private static readonly Lazy<UIManager> _instance = new(() => new UIManager());

        public static UIManager Instance => _instance.Value;

        private readonly UIStack _stack = new UIStack();

        private readonly Dictionary<UIPanelType, BaseUIPanel> _activePanels = new Dictionary<UIPanelType, BaseUIPanel>();
        private readonly Dictionary<UIPanelType, MonoPanel> _activeMonoPanels = new Dictionary<UIPanelType, MonoPanel>();
        private readonly Dictionary<UIPanelType, BasePanelModel> _activePanelModels = new Dictionary<UIPanelType, BasePanelModel>();

        private UIManager()
        {
        }

        public void ShowPanel(UIPanelType type, params object[] args)
        {
            if (_activePanels.ContainsKey(type))
            {
                Log.LogWarning($"Panel {type} is already active");
                return;
            }
            var config = UIPanelConfigRegistry.GetConfig(type);
            var monoPanel = UIPanelFactory.Create(type);
            var panelModel = UIPanelModelFactory.Create(type);
            var panel = UIPresenterFactory.Create(type, monoPanel, panelModel);
            var panelData = new UIPanelData(type, panel.IsModal);
            if (config?.IsModal == true)
            {
                UICanvasManager.Instance?.ShowModalBackground();
            }
            _stack.Push(panelData);
            _activePanels.Add(type, panel);
            _activeMonoPanels.Add(type, monoPanel);
            panel.OnEnter(args);
        }

        public void HidePanel(UIPanelType type)
        {
            if (!_activePanels.TryGetValue(type, out var panel))
            {
                Log.LogWarning($"Panel {type} is not active");
                return;
            }

            if (!_activeMonoPanels.TryGetValue(type, out var monoPanel))
            {
                Log.LogError($"Panel {type} does not have a MonoPanel component");
                return;
            }
            
            var config = UIPanelConfigRegistry.GetConfig(type);
            
            panel.OnExit();
            var panelData = FindPanelInStack(type);
            if (panelData != null)
            {
                RemovePanelFromStack(type);
            }
            _activePanels.Remove(type);
            _activeMonoPanels.Remove(type);
            UIAssetLoader.Destroy(monoPanel.gameObject);
            
            if (config?.IsModal == true)
            {
                UICanvasManager.Instance?.HideModalBackground();
            }
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
    }
}
