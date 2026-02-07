using System;
using System.Collections.Generic;
using UIModule.Data.Models;
using UnityEngine;

namespace UIModule.Interfaces
{
    public interface IBagView
    {
        void ShowView();
        void HideView();
        event Action OnCloseClick;
    }
}
