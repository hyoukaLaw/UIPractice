using System;
using System.Collections.Generic;
using UIModule.Data.Models;
using UnityEngine;

namespace UIModule.Interfaces
{
    public interface IBagView
    {
        void SetSelectedItemIndex(int index);
        void ShowView();
        void HideView();
        event Action OnCloseClick;
        event Action<int> OnItemClick;
    }
}
