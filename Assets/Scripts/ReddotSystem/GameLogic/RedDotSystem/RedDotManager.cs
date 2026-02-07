/*
 * Description:             RedDotManager.cs
 * Author:                  TONYTANG
 * Create Date:             2022/08/14
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RedDotManager.cs
/// 红点管理单例类
/// </summary>
public class RedDotManager : SingletonTemplate<RedDotManager>
{
    /// <summary>
    /// 标脏的红点运算单元Map<红点运算单元, 红点运算单元>
    /// </summary>
    private Dictionary<RedDotUnit, RedDotUnit> mDirtyRedDotUnitMap;

    private HashSet<RedDotUnitWithId> _dirtyRedDotUnitWithId = new(); // 标记脏的带Id的红点计算单元

    private Dictionary<RedDotUnit, bool> mCaculatedRedDotUnitResultChangeMap; // 脏的红点单元，重新计算后结果确实有变化，存进这个容器

    /// <summary>
    /// 标脏检测更新帧率
    /// </summary>
    private const int DIRTY_UPDATE_INTERVAL_FRAME = 10;

    /// <summary>
    /// 经历的帧数
    /// </summary>
    private int mFramePassed;

    public RedDotManager()
    {
        mFramePassed = 0;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        InitRedDotData();
    }

    /// <summary>
    /// 初始化红点数据
    /// </summary>
    private void InitRedDotData()
    {
        mDirtyRedDotUnitMap = new Dictionary<RedDotUnit, RedDotUnit>();
        mCaculatedRedDotUnitResultChangeMap = new Dictionary<RedDotUnit, bool>();
    }

    /// <summary>
    /// 执行所有红点名运算单元运算
    /// Note:
    /// 进入游戏后获取完相关数据后触发一次,确保第一次运算结果缓存
    /// </summary>
    public void DoAllRedDotUnitCaculate()
    {
        var redDotUnitInfoMap = RedDotModel.Singleton.GetRedDotUnitInfoMap();
        foreach(var redDotUnitInfo in redDotUnitInfoMap)
        {
            if(redDotUnitInfo.Value.SupportIdParameter)
                DoRedDotUnitCalculateWithId(redDotUnitInfo.Key);
            else
                DoRedDotUnitCalculate(redDotUnitInfo.Key);
        }
    }

    /// <summary>
    /// 触发红点名刷新回调
    /// </summary>
    /// <param name="redDotName"></param>
    public bool TriggerRedDotNameUpdate(string redDotName)
    {
        var redDotInfo = RedDotModel.Singleton.GetRedDotInfoByName(redDotName);
        if(redDotInfo == null)
        {
            Debug.LogError($"触发红点名:{redDotName}刷新回调失败!");
            return false;
        }
        var redDotNameResult = GetRedDotNameResult(redDotName);
        redDotInfo.TriggerUpdate(redDotNameResult.result, redDotNameResult.redDotType);
        return true;
    }

    public bool TriggerRedDotNameUpdateWithId(string redDotName)
    {
        var redDotInfo = RedDotModel.Singleton.GetRedDotInfoByName(redDotName);
        if(redDotInfo == null)
        {
            Debug.LogError($"触发红点名:{redDotName}刷新回调失败!");
            return false;
        }
        var redDotNameResult = GetRedDotNameResultWithId(redDotName);
        redDotInfo.TriggerUpdate(redDotNameResult.result, redDotNameResult.redDotType);
        return true;
    }

    private void TriggerRedDotNameUpdate(Dictionary<string, string> resultChangedRedDotNameMap)
    {
        foreach (var resultChangedRedDotName in resultChangedRedDotNameMap)
        {
            TriggerRedDotNameUpdate(resultChangedRedDotName.Key);
        }
    }

    private void TriggerRedDotNameUpdateWithId(Dictionary<string, string> resultChangedRedDotNameMap)
    {
        foreach (var resultChangedRedDotName in resultChangedRedDotNameMap)
        {
            TriggerRedDotNameUpdateWithId(resultChangedRedDotName.Key);
        }   
    }


    /// <summary>
    /// 获取指定红点名的结果数据
    /// </summary>
    /// <param name="redDotName"></param>
    /// <returns></returns>
    public (int result, RedDotType redDotType) GetRedDotNameResult(string redDotName)
    {
        (int result, RedDotType redDotType) redDotNameResult = (0, RedDotType.NONE);
        redDotNameResult.result = 0;
        var redDotUnitList = RedDotModel.Singleton.GetRedDotUnitsByName(redDotName);
        if (redDotUnitList != null)
        {
            var result = 0;
            var redDotType = RedDotType.NONE;
            foreach (var redDotUnit in redDotUnitList)
            {
                var redDotUnitResult = RedDotModel.Singleton.GetRedDotUnitResult(redDotUnit);
                if (redDotUnitResult > 0)
                {
                    var redDotType2 = RedDotModel.Singleton.GetRedDotUnitRedType(redDotUnit);
                    redDotType = redDotType | redDotType2;
                }
                result += redDotUnitResult;
            }
            redDotNameResult.result = result;
            redDotNameResult.redDotType = redDotType;
        }
        
        return redDotNameResult;
    }

    public (int result, RedDotType redDotType) GetRedDotNameResultWithId(string redDotName)
    {
        (int result, RedDotType redDotType) redDotNameResult = (0, RedDotType.NONE);
        int nameResult = RedDotModel.Singleton.GetRedDotNameResult(redDotName);
        redDotNameResult.result = nameResult;
        var redDotUnitList = RedDotModel.Singleton.GetRedDotUnitsByName(redDotName);
        if (redDotUnitList != null)
        {
            var redDotType = RedDotType.NONE;
            foreach (var redDotUnit in redDotUnitList)
            {
                var redDotUnitResult = RedDotModel.Singleton.GetRedDotUnitResult(redDotUnit);
                if (redDotUnitResult > 0)
                {
                    var redDotType2 = RedDotModel.Singleton.GetRedDotUnitRedType(redDotUnit);
                    redDotType = redDotType | redDotType2;
                }
            }
            redDotNameResult.redDotType = redDotType;
        }
        return redDotNameResult;
    }

    /// <summary>
    /// 绑定指定红点名刷新回调
    /// </summary>
    /// <param name="redDotName"></param>
    /// <param name="refreshDelegate"></param>
    /// <returns></returns>
    public void BindRedDotName(string redDotName, Action<string, int, RedDotType> refreshDelegate)
    {
        var redDotInfo = RedDotModel.Singleton.GetRedDotInfoByName(redDotName);
        if(redDotInfo == null)
        {
            Debug.LogError($"找不到红点名:{redDotName}红点信息,绑定刷新失败!");
            return;
        }
        redDotInfo.Bind(refreshDelegate);
    }

    /// <summary>
    /// 解绑定指定红点名
    /// </summary>
    /// <param name="redDotName"></param>
    /// <param name="refreshDelegate"></param>
    /// <returns></returns>
    public void UnbindRedDotName(string redDotName, Action<string, int, RedDotType> refreshDelegate)
    {
        var redDotInfo = RedDotModel.Singleton.GetRedDotInfoByName(redDotName);
        if (redDotInfo == null)
        {
            Debug.LogError($"找不到红点名:{redDotName}红点信息,解绑定失败!");
            return;
        }
        redDotInfo.UnBind(refreshDelegate);
    }

    /// <summary>
    /// 获取指定红点名结果
    /// </summary>
    /// <param name="redDotName"></param>
    /// <returns></returns>
    public int GetRedDotResult(string redDotName)
    {
        int result = RedDotModel.Singleton.GetRedDotNameResult(redDotName);
        if (result != 0)
        {
            return result;
        }
        var redDotUnitList = RedDotModel.Singleton.GetRedDotUnitsByName(redDotName);
        if(redDotUnitList == null)
        {
            return 0;
        }
        result = 0;
        foreach(var redDotUnit in redDotUnitList)
        {
            var redDotUnitResult = RedDotModel.Singleton.GetRedDotUnitResult(redDotUnit);
            result += redDotUnitResult;
        }
        return result;
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
        if(!RedDotModel.Singleton.IsInitCompelte)
        {
            return;
        }
        mFramePassed++;
        if(mFramePassed >= DIRTY_UPDATE_INTERVAL_FRAME)
        {
            CheckDirtyRedDotUnit();
            CheckDirtyRedDotUnitWithId();
            mFramePassed = 0;
        }
    }

    /// <summary>
    /// 标记指定红点运算类型脏
    /// </summary>
    /// <param name="redDotUnit"></param>
    public void MarkRedDotUnitDirty(RedDotUnit redDotUnit)
    {
        if (!mDirtyRedDotUnitMap.ContainsKey(redDotUnit))
        {
            mDirtyRedDotUnitMap.Add(redDotUnit, redDotUnit);
        }
    }

    public void MarkRedDotUnitDirty(RedDotUnit redDotUnit, int id)
    {
        if(!_dirtyRedDotUnitWithId.Contains(new RedDotUnitWithId(redDotUnit, id)))
             _dirtyRedDotUnitWithId.Add(new RedDotUnitWithId(redDotUnit, id));
    }

    /// <summary>
    /// 检查标脏红点运算单元
    /// </summary>
    private void CheckDirtyRedDotUnit()
    {
        Dictionary<string, string> resultChangedRedDotNameMap = GetResultChangedRedDotNameMap();
        mDirtyRedDotUnitMap.Clear();
        TriggerRedDotNameUpdate(resultChangedRedDotNameMap); // 对确实有结果变化的来进行UI更新
    }

    private Dictionary<string, string> GetResultChangedRedDotNameMap() // 获取实际有变化的红点名
    {
        mCaculatedRedDotUnitResultChangeMap.Clear();
        var resultChangedRedDotNameMap = new Dictionary<string, string>();
        foreach(var dirtyRedDotUnit in mDirtyRedDotUnitMap)
        {
            var realRedDotUnit = dirtyRedDotUnit.Key;
            var redDotNameList = RedDotModel.Singleton.GetRedDotNamesByUnit(realRedDotUnit); // unit关联到name，计算name的最终结果是否变化
            if(redDotNameList == null || redDotNameList.Count == 0)
            {
                continue;
            }
            foreach (var redDotName in redDotNameList)
            {
                var redDotUnitList = RedDotModel.Singleton.GetRedDotUnitsByName(redDotName);
                if(redDotUnitList == null || redDotUnitList.Count == 0)
                {
                    continue;
                }
                foreach(var redDotUnit in redDotUnitList)
                {
                    bool resultChange = false;
                    if(!mCaculatedRedDotUnitResultChangeMap.TryGetValue(redDotUnit, out resultChange))
                    {
                        resultChange = DoRedDotUnitCalculate(redDotUnit);
                        mCaculatedRedDotUnitResultChangeMap.Add(redDotUnit, resultChange);
                    }
                    if(!resultChange)
                    {
                        continue;
                    }
                    if(!resultChangedRedDotNameMap.ContainsKey(redDotName))
                    {
                        resultChangedRedDotNameMap.Add(redDotName, redDotName);
                    }
                }
            }
        }
        return resultChangedRedDotNameMap;
    }

    private void CheckDirtyRedDotUnitWithId()
    {
        var resultChangedRedDotNameMap = new Dictionary<string, string>();
        foreach (var dirtyRedDotUnitWithId in _dirtyRedDotUnitWithId)
        {
            var redDotUnit = dirtyRedDotUnitWithId.RedDotUnit;
            var id = dirtyRedDotUnitWithId.Id;
            {
                var names = RedDotModel.Singleton.GetRedDotNameByUnitWithId(redDotUnit, id);
                foreach (var nmWithId in names)
                {
                    string nm = nmWithId.RedDotName;
                    DoRedDotNameCalculateWithId(nm, id);
                    resultChangedRedDotNameMap.TryAdd(nm, nm);
                }
            }
        }
        TriggerRedDotNameUpdateWithId(resultChangedRedDotNameMap);
        _dirtyRedDotUnitWithId.Clear();
    }

    private bool DoRedDotUnitCalculate(RedDotUnit redDotUnit)
    {
        var preResult = RedDotModel.Singleton.GetRedDotUnitResult(redDotUnit);
        var redDotUnitFunc = RedDotModel.Singleton.GetRedDotUnitFunc(redDotUnit);
        var result = 0;
        if(redDotUnitFunc != null)
        {
            result = redDotUnitFunc();
        }
        else
        {
            Debug.LogError($"红点运算单元:{redDotUnit.ToString()}未绑定有效计算方法!");
        }
        RedDotModel.Singleton.SetRedDotUnitResult(redDotUnit, result);
        return preResult != result;
    }

    private void DoRedDotUnitCalculateWithId(RedDotUnit redDotUnit)
    {
        var reddotNameList = RedDotModel.Singleton.GetRedDotNamesByUnit(redDotUnit);
        if(reddotNameList == null || reddotNameList.Count == 0)
        {
            return ;
        }
        foreach(var redDotName in reddotNameList)
        {
            DoRedDotNameCalculateWithId(redDotName, 0);
        }
        return ;
    }

    private bool DoRedDotNameCalculateWithId(string redDotName, int id)  // redDotName是包含Id的完整红点名
    {
        var redDotUnitList = RedDotModel.Singleton.GetRedDotUnitsByName(redDotName);
        if (redDotUnitList == null || redDotUnitList.Count == 0)
        {
            return false;
        }
        var totalResult = 0;
        foreach (var redDotUnit in redDotUnitList)
        {
            var redDotUnitInfo = RedDotModel.Singleton.GetRedDotUnitInfo(redDotUnit);
            if (redDotUnitInfo != null && redDotUnitInfo.SupportIdParameter)
            {
                if (redDotUnitInfo.RedDotUnitCalculateFuncWithId != null)
                {
                    totalResult += redDotUnitInfo.RedDotUnitCalculateFuncWithId(id);
                }
            }
            else
            {
                var func = RedDotModel.Singleton.GetRedDotUnitFunc(redDotUnit);
                if (func != null)
                {
                    totalResult += func();
                }
            }
        }
        var preResult = RedDotModel.Singleton.GetRedDotNameResult(redDotName);
        RedDotModel.Singleton.SetRedDotNameResult(redDotName, totalResult);
        return preResult != totalResult;
    }
}