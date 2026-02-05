namespace UIModule.Data
{
    public class UIPanelConfig
    {
        public UIPanelType Type { get; set; }
        public string PrefabPath { get; set; }
        public UILayer Layer { get; set; }
        public bool IsModal { get; set; }

        public UIPanelConfig()
        {
            Layer = UILayer.Normal;
            IsModal = false;
        }
    }

    public static class UIPanelConfigRegistry
    {
        private static readonly System.Collections.Generic.Dictionary<UIPanelType, UIPanelConfig> _configs = 
            new System.Collections.Generic.Dictionary<UIPanelType, UIPanelConfig>();

        static UIPanelConfigRegistry()
        {
            Register(new UIPanelConfig
            {
                Type = UIPanelType.Main,
                PrefabPath = "Prefabs/UI/MainPanel",
                Layer = UILayer.Normal,
                IsModal = false
            });

            Register(new UIPanelConfig
            {
                Type = UIPanelType.Character,
                PrefabPath = "Prefabs/UI/CharacterPanel",
                Layer = UILayer.Normal,
                IsModal = false
            });
            
            Register(new UIPanelConfig
            {
                Type = UIPanelType.CharacterStory,
                PrefabPath = "Prefabs/UI/CharacterStoryPanel",
                Layer = UILayer.Normal,
                IsModal = false
            });
        }

        public static void Register(UIPanelConfig config)
        {
            if (!_configs.ContainsKey(config.Type))
            {
                _configs.Add(config.Type, config);
            }
        }

        public static UIPanelConfig GetConfig(UIPanelType type)
        {
            _configs.TryGetValue(type, out var config);
            return config;
        }
    }
}
