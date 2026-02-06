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
            string redDotName = string.Format(RedDotNames.CHARACTER_STORY_TEMPLATE, i);
            //RedDotModel.Singleton.RegisterDynamicRedDot(redDotName, $"角色{i}故事红点", RedDotUnit.CHARACTER_STORY_NEW);
            RedDotManager.Singleton.MarkRedDotNameDirty(redDotName);
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
            string redDotName = string.Format(RedDotNames.CHARACTER_STORY_TEMPLATE, i);
            RedDotModel.Singleton.RegisterDynamicRedDot(redDotName, $"角色{i}故事红点", RedDotUnit.CHARACTER_STORY_NEW);
        }
    }
    
    private void UnregisterDynamicRedDot()
    {
        for (int i = 0; i < 4; i++)
        {
            string redDotName = string.Format(RedDotNames.CHARACTER_STORY_TEMPLATE, i);
            RedDotModel.Singleton.UnregisterDynamicRedDot(redDotName);
        }
    }
}
