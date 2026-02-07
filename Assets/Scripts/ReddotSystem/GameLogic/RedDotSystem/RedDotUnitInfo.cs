/*
 * Description:             RedDotUnitInfo.cs
 * Author:                  TONYTANG
 * Create Date:             2022/08/14
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RedDotUnitCalculateMode
{
    LEAF,
    COMPOSITE,
}

public enum RedDotUnitAggregateMode
{
    SUM,
    ANY_POSITIVE,
}

/// <summary>
/// RedDotUnitInfo.cs
/// 红点运算单元信息类
/// </summary>
public class RedDotUnitInfo
{
    /// <summary>
    /// 红点运算单元类型
    /// </summary>
    public RedDotUnit RedDotUnit
    {
        get;
        private set;
    }

    /// <summary>
    /// 红点运算单元描述
    /// </summary>
    public string RedDotUnitDes
    {
        get;
        private set;
    }

    /// <summary>
    /// 红点运算单元对应显示的红点类型
    /// </summary>
    public RedDotType RedDotType
    {
        get;
        private set;
    }

    /// <summary>
    /// 红点运算单元对应红点计算回调
    /// </summary>
    public Func<int> RedDotUnitCalculateFunc
    {
        get;
        private set;
    }

    public bool SupportIdParameter
    {
        get;
        private set;
    }

    public Func<int, int> RedDotUnitCalculateFuncWithId
    {
        get;
        private set;
    }

    public RedDotUnitCalculateMode CalculateMode
    {
        get;
        private set;
    }

    public RedDotUnitAggregateMode AggregateMode
    {
        get;
        private set;
    }

    public List<RedDotUnit> DependencyUnits
    {
        get;
        private set;
    }

    private RedDotUnitInfo()
    {

    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="redDotUnit">红点运算单元类型</param>
    /// <param name="redDotUnitDes">红点运算单元描述</param>
    /// <param name="redDotUnitCalculateFunc">红点运算单元计算方法</param>
    /// <param name="redDotType">红点显示类型</param>
    public RedDotUnitInfo(RedDotUnit redDotUnit, string redDotUnitDes, Func<int> redDotUnitCalculateFunc, RedDotType redDotType)
    {
        RedDotUnit = redDotUnit;
        RedDotUnitDes = redDotUnitDes;
        RedDotUnitCalculateFunc = redDotUnitCalculateFunc;
        RedDotType = redDotType;
        SupportIdParameter = false;
        CalculateMode = RedDotUnitCalculateMode.LEAF;
        AggregateMode = RedDotUnitAggregateMode.SUM;
        DependencyUnits = null;
    }

    public RedDotUnitInfo(RedDotUnit redDotUnit, string redDotUnitDes, Func<int, int> redDotUnitCalculateFunc, RedDotType redDotType)
    {
        RedDotUnit = redDotUnit;
        RedDotUnitDes = redDotUnitDes;
        RedDotUnitCalculateFuncWithId = redDotUnitCalculateFunc;
        RedDotType = redDotType;
        SupportIdParameter = true;
        CalculateMode = RedDotUnitCalculateMode.LEAF;
        AggregateMode = RedDotUnitAggregateMode.SUM;
        DependencyUnits = null;
    }

    public RedDotUnitInfo(RedDotUnit redDotUnit, string redDotUnitDes, List<RedDotUnit> dependencyUnits, RedDotType redDotType, RedDotUnitAggregateMode redDotUnitAggregateMode)
    {
        RedDotUnit = redDotUnit;
        RedDotUnitDes = redDotUnitDes;
        RedDotType = redDotType;
        SupportIdParameter = false;
        CalculateMode = RedDotUnitCalculateMode.COMPOSITE;
        AggregateMode = redDotUnitAggregateMode;
        DependencyUnits = dependencyUnits;
    }
}
