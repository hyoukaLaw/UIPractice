using UIModule.Data;
using UIModule.Core;
using UIModule.Interfaces;

namespace UIModule.Panels
{
    public class CharacterStoryPanel : BaseUIPanel
    {
        private string storyId;
        private string chapterId;
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
                storyId = args[0]?.ToString();
            }

            if (args.Length > 1 && args[1] != null)
            {
                chapterId = args[1]?.ToString();
            }
            _characterStoryView.OnCloseClick += CloseCurrent;
            Log.LogInfo($"CharacterStoryPanel OnEnter: StoryId={storyId}, ChapterId={chapterId}");
        }

        public override void OnExit()
        {
            _characterStoryView.OnCloseClick -= CloseCurrent;
            Log.LogInfo($"CharacterStoryPanel OnExit: StoryId={storyId}");
        }

        public override void OnPause()
        {
            Log.LogInfo($"CharacterStoryPanel OnPause: StoryId={storyId}");
        }

        public override void OnResume()
        {
            Log.LogInfo($"CharacterStoryPanel OnResume: StoryId={storyId}");
        }

        public void SetStoryData(string storyId, string chapterId)
        {
            this.storyId = storyId;
            this.chapterId = chapterId;
        }

        private void CloseCurrent()
        {
            UIManager.Instance.HidePanel(UIPanelType.CharacterStory);
        }
    }
}
