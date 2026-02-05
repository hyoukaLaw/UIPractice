using UIModule.Core;
using UIModule.Data;
using UIModule.Data.Models;
using UIModule.Interfaces;
using UIModule.Panels;

namespace UIModule.Adapters
{
    public static class UIPresenterFactory
    {
        public static BaseUIPanel Create(UIPanelType type, MonoPanel view, BasePanelModel model)
        {
            switch (type)
            {
                case UIPanelType.Character:
                    return view is ICharacterView characterView ? new CharacterPanel(characterView, model) : null;
                case UIPanelType.CharacterStory:
                    return view is ICharacterStoryView characterStoryView ? new CharacterStoryPanel(characterStoryView) : null;
                case UIPanelType.Main:
                    return view is IMainView mainView ? new MainPanel(mainView) : null;
                case UIPanelType.ModalConfirm:
                    return view is IModalView modalViewConfirm ? new ModalPanel(modalViewConfirm, model) : null;
                case UIPanelType.ModalAlert:
                    return view is IModalView modalViewAlert ? new AlertPanel(modalViewAlert, model) : null;
                default:
                    return null;
            }
        }
    }
}
