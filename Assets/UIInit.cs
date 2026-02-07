using System;
using System.Collections;
using System.Collections.Generic;
using UIModule.Core;
using UIModule.Data;
using UnityEngine;

public class UIInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RedDotModel.Singleton.Init();
        RedDotManager.Singleton.Init();
        // 所有数据初始化完成后触发一次红点运算单元计算
        RegisterDynamicRedDot();
        RedDotManager.Singleton.DoAllRedDotUnitCaculate();
        UIManager.Instance.ShowPanel(UIPanelType.Main);
    }

    // Update is called once per frame
    void Update()
    {
        RedDotManager.Singleton.Update();
        for (int i = 0; i < 4; i++)
        {
            RedDotManager.Singleton.MarkRedDotUnitDirty(RedDotUnit.CHARACTER_STORY_NEW, i);
            RedDotManager.Singleton.MarkRedDotUnitDirty(RedDotUnit.CHARACTER_CG_NEW, i);
        }
    }

    private void OnDestroy()
    {
        UnregisterDynamicRedDot();
    }

    private void RegisterDynamicRedDot()
    {
        for (int i = 0; i < 4; i++)
        {
            string redDotNameCharacter = string.Format(RedDotNames.CHARACTER_ID_TEMPLATE, i);
            //RedDotModel.Singleton.RegisterDynamicRedDot(i, redDotNameCharacter, $"角色{i}红点", RedDotUnit.CHARACTER_STORY_NEW);
            RedDotModel.Singleton.RegisterDynamicRedDot(i, redDotNameCharacter, $"角色{i}红点", new List<RedDotUnit>(){RedDotUnit.CHARACTER_STORY_NEW, RedDotUnit.CHARACTER_CG_NEW});
            string redDotNameStory = string.Format(RedDotNames.CHARACTER_STORY_ID_TEMPLATE, i);
            RedDotModel.Singleton.RegisterDynamicRedDot(i, redDotNameStory, $"角色{i}故事红点", RedDotUnit.CHARACTER_STORY_NEW);
            string redDotNameCg = string.Format(RedDotNames.CHARACTER_CG_ID_TEMPLATE, i);
            RedDotModel.Singleton.RegisterDynamicRedDot(i, redDotNameCg, $"角色{i}Cg红点", RedDotUnit.CHARACTER_CG_NEW);
        }
    }
    
    private void UnregisterDynamicRedDot()
    {
        for (int i = 0; i < 4; i++)
        {
            string redDotNameStory = string.Format(RedDotNames.CHARACTER_STORY_ID_TEMPLATE, i);
            RedDotModel.Singleton.UnregisterDynamicRedDot(redDotNameStory, i);
            string redDotNameCg = string.Format(RedDotNames.CHARACTER_CG_ID_TEMPLATE, i);
            RedDotModel.Singleton.UnregisterDynamicRedDot(redDotNameCg, i);
        }
    }
}
