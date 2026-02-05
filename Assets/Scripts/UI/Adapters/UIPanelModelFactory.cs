using UIModule.Core;
using UIModule.Data;
using UIModule.Data.Models;

namespace UIModule.Adapters
{
    public static class UIPanelModelFactory
    {
        public static BasePanelModel Create(UIPanelType type, params object[] args)
        {
            switch (type)
            {
                case UIPanelType.Character:
                    return CreateCharacterPanelModel(args);
                case UIPanelType.CharacterStory:
                case UIPanelType.Main:
                    //return new BasePanelModel { PanelType = UIPanelType.Main };
                default:
                    Log.LogWarning($"No model defined for panel type: {type}");
                    return null;
            }
        }

        private static CharacterPanelModel CreateCharacterPanelModel(object[] args)
        {
            var model = new CharacterPanelModel();
            return model;
        }
    }
}