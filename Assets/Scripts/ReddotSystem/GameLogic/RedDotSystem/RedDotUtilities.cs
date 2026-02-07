/*
 * Description:             RedDotUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2022/08/14
 */

using System.Collections;
using System.Collections.Generic;
using UIModule.Core;
using UIModule.Core.UISystem;
using UIModule.Data;
using UIModule.Data.Models;
using UnityEngine;

/// <summary>
/// RedDotUtilities.cs
/// 红点工具类
/// </summary>
public static class RedDotUtilities
{
    #region 红点辅助方法部分
    /// <summary>
    /// 获取指定红点数量和类型的文本显示
    /// Note:
    /// 红点显示类型优先级:
    /// 新 > 纯数字 > 纯红点
    /// </summary>
    /// <param name="result"></param>
    /// <param name="redDotType"></param>
    /// <returns></returns>
    public static string GetRedDotResultText(int result, RedDotType redDotType)
    {
        if(result <= 0)
        {
            return string.Empty;
        }
        var redDotText = string.Empty;
        if((redDotType & RedDotType.NEW) != RedDotType.NONE)
        {
            redDotText = "新";
        }
        else if((redDotType & RedDotType.NUMBER) != RedDotType.NONE)
        {
            redDotText = result.ToString();
        }
        return redDotText;
    }
    #endregion

    #region 红点数据以及逻辑运算部分

    #region 人物相关
    public static int CalculateCharacterNew()
    {
        int result = 0;
        foreach (var data in InitialData.Singleton.CharacterConfig.GetCharacters())
        {
            if (data.GetHasNewStory() || data.GetHasNewCg())
            {
                result++;
                break;
            }
        }
        return result;
    }

    public static int CalculateCharacterStoryNew(int characterId)
    {
        var characterData = InitialData.Singleton.CharacterConfig.GetCharacterById(characterId);
        return characterData.GetHasNewStory() ? 1 : 0;
    }

    public static int CalculateCharacterCgNew(int characterId)
    {
        var characterData = InitialData.Singleton.CharacterConfig.GetCharacterById(characterId);
        return characterData.GetHasNewCg() ? 1 : 0;
    }

    public static int CalculateCharacterNew(int characterId)
    {
        var characterData = InitialData.Singleton.CharacterConfig.GetCharacterById(characterId);
        return characterData.GetHasNewCg() || characterData.GetHasNewStory() ? 1 : 0;
    }
    
    
    
    #endregion
    
    
    #endregion
}
