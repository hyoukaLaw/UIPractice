using UIModule.Data;
using UIModule.Core;
using UIModule.Interfaces;

namespace UIModule.Panels
{
    public class CharacterStoryPanel : BaseUIPanel
    {
        private string _storyId;
        private string _chapterId;
        private ICharacterStoryView _characterStoryView;

        public CharacterStoryPanel(ICharacterStoryView characterStoryView)
        {
            PanelType = UIPanelType.CharacterStory;
            IsModal = false;
            _characterStoryView = characterStoryView;
        }

        public override void OnEnter(params object[] args)
        {
            if (args.Length > 0)
            {
                _storyId = args[0]?.ToString();
            }

            if (args.Length > 1 && args[1] != null)
            {
                _chapterId = args[1]?.ToString();
            }
            _characterStoryView.OnCloseClick += CloseCurrent;
            Log.LogInfo($"CharacterStoryPanel OnEnter: StoryId={_storyId}, ChapterId={_chapterId}");
        }

        public override void OnExit()
        {
            _characterStoryView.OnCloseClick -= CloseCurrent;
            Log.LogInfo($"CharacterStoryPanel OnExit: StoryId={_storyId}");
        }

        public override void OnPause()
        {
            Log.LogInfo($"CharacterStoryPanel OnPause: StoryId={_storyId}");
        }

        public override void OnResume()
        {
            Log.LogInfo($"CharacterStoryPanel OnResume: StoryId={_storyId}");
        }

        private void CloseCurrent()
        {
            UIManager.Instance.HidePanel(UIPanelType.CharacterStory);
        }
    }
}
