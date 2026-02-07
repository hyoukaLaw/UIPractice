/*
 * Description:             RedDotNames.cs
 * Author:                  TONYTANG
 * Create Date:             2022/08/14
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RedDotNames.cs
/// 红点名
/// Note:
/// 红点名采用前缀树风格定义父子关系(采用|分割父子关系)
/// 所有游戏里的单个静态红点都在这里一一定义对应
/// </summary>
public static class RedDotNames
{
    #region 主界面红点部分
    /// <summary>
    /// 主界面人物按钮红点
    /// </summary>
    public const string MAIN_UI_CHARACTER = "MAIN_UI_CHARACTER";
    #endregion

    #region ID-based红点模板
    public const string CHARACTER_ID_TEMPLATE = "CHARACTER|{0}";
    public const string CHARACTER_STORY_ID_TEMPLATE = "CHARACTER|STORY|{0}";
    public const string CHARACTER_CG_ID_TEMPLATE = "CHARACTER|CG|{0}";
    #endregion
}