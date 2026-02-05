using UIModule.Core;
using UIModule.Data;
using UIModule.Data.Models;

namespace UIModule.Adapters
{
    public static class UIPanelModelFactory
    {
        public static BasePanelModel Create(UIPanelType type)
        {
            BasePanelModel model;
            switch (type)
            {
                case UIPanelType.Character:
                    return new CharacterPanelModel();
                case UIPanelType.CharacterStory:
                case UIPanelType.Main:
                case UIPanelType.ModalConfirm:
                    model = new ModalPanelModel();
                    return model;
                case UIPanelType.ModalAlert:
                    model = new AlertPanelModel();
                    return model;
                default:
                    Log.LogWarning($"No model defined for panel type: {type}");
                    return null;
            }
        }
    }
}