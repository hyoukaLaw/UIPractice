using System;
using UIModule.Data;

namespace UIModule.Core
{
    public abstract class BaseUIPanel
    {
        public abstract void OnEnter(params object[] args);
        public abstract void OnExit();
        public abstract void OnPause();
        public abstract void OnResume();

        public bool IsModal { get; protected set; }
        public UIPanelType PanelType { get; protected set; }
    }
}
