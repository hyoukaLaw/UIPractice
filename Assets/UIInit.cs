using System.Collections.Generic;
using UIModule.Core;
using UIModule.Core.UISystem;
using UIModule.Data;
using UnityEngine;

public class UIInit : MonoBehaviour
{
    void Start()
    {
        RedDotModel.Singleton.Init();
        RedDotManager.Singleton.Init();
        RegisterDynamicRedDot();
        RedDotManager.Singleton.DoAllRedDotUnitCalculate();
        UIManager.Instance.ShowPanel(UIPanelType.Main);
    }

    void Update()
    {
        RedDotManager.Singleton.Update();
    }

    private void OnDestroy()
    {
        UnregisterDynamicRedDot();
    }

    private void RegisterDynamicRedDot()
    {
        foreach (var characterId in GetCharacterIds())
        {
            string redDotNameCharacter = string.Format(RedDotNames.CHARACTER_ID_TEMPLATE, characterId);
            RedDotModel.Singleton.RegisterDynamicRedDot(characterId, redDotNameCharacter, $"角色{characterId}红点", new List<RedDotUnit>() { RedDotUnit.CHARACTER_STORY_NEW, RedDotUnit.CHARACTER_CG_NEW });
            string redDotNameStory = string.Format(RedDotNames.CHARACTER_STORY_ID_TEMPLATE, characterId);
            RedDotModel.Singleton.RegisterDynamicRedDot(characterId, redDotNameStory, $"角色{characterId}故事红点", RedDotUnit.CHARACTER_STORY_NEW);
            string redDotNameCg = string.Format(RedDotNames.CHARACTER_CG_ID_TEMPLATE, characterId);
            RedDotModel.Singleton.RegisterDynamicRedDot(characterId, redDotNameCg, $"角色{characterId}Cg红点", RedDotUnit.CHARACTER_CG_NEW);
        }
    }

    private void UnregisterDynamicRedDot()
    {
        foreach (var characterId in GetCharacterIds())
        {
            string redDotNameCharacter = string.Format(RedDotNames.CHARACTER_ID_TEMPLATE, characterId);
            RedDotModel.Singleton.UnregisterDynamicRedDot(redDotNameCharacter, characterId);
            string redDotNameStory = string.Format(RedDotNames.CHARACTER_STORY_ID_TEMPLATE, characterId);
            RedDotModel.Singleton.UnregisterDynamicRedDot(redDotNameStory, characterId);
            string redDotNameCg = string.Format(RedDotNames.CHARACTER_CG_ID_TEMPLATE, characterId);
            RedDotModel.Singleton.UnregisterDynamicRedDot(redDotNameCg, characterId);
        }
    }

    private List<int> GetCharacterIds()
    {
        var characterIds = new List<int>();
        var characterConfig = InitialData.Singleton.CharacterConfig;
        if (characterConfig == null || characterConfig.GetCharacters() == null)
        {
            return characterIds;
        }

        foreach (var character in characterConfig.GetCharacters())
        {
            if (character == null)
            {
                continue;
            }
            characterIds.Add(character.GetId());
        }
        return characterIds;
    }
}
