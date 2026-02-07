using System;

namespace UIModule.Panels
{
    public interface IMainView
    {
        void SetCharacterRedDot(bool show);
        event Action OnCharacterClick;
        event Action OnBagClick;
    }
}
