using System;

namespace UIModule.Interfaces
{
    public interface IModalView
    {
        void SetTitle(string title);
        void SetMessage(string message);
        void ShowView();
        void HideView();
        event Action OnConfirmClick;
        event Action OnCancelClick;
    }
}
