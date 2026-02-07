using UIModule.Core;
using UIModule.Data;
using UIModule.Data.Models;

namespace UIModule.Adapters
{
    public static class UIPanelModelFactory
    {
        public static BasePanelModel Create(UIPanelType type)
        {
            switch (type)
            {
                case UIPanelType.Character:
                    return new CharacterPanelModel();
                case UIPanelType.CharacterStory:
                    return new CharacterStoryPanelModel();
                case UIPanelType.Main:
                    return new MainPanelModel();
                case UIPanelType.ModalConfirm:
                    return new ModalPanelModel();
                case UIPanelType.ModalAlert:
                    return new AlertPanelModel();
                case UIPanelType.Bag:
                    return new BagPanelModel();
                default:
                    Log.LogWarning($"No model defined for panel type: {type}");
                    return null;
            }
        }
    }
}
