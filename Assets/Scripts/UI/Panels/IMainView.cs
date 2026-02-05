using System;

namespace UIModule.Panels
{
    public interface IMainView
    {
        event Action OnCharacterClick;
        event Action OnBagClick;
    }
}