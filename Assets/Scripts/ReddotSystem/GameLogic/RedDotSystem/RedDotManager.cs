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
    /// 标脏的红点运算单元集合
    /// </summary>
    private HashSet<RedDotUnit> _dirtyRedDotUnitSet;

    /// <summary>
    /// 标脏的带Id红点运算单元集合
    /// </summary>
    private HashSet<RedDotUnitWithId> _dirtyRedDotUnitWithId = new();

    /// <summary>
    /// 标脏检测更新帧率
    /// </summary>
    private const int _dirtyUpdateIntervalFrame = 10;

    /// <summary>
    /// 经历的帧数
    /// </summary>
    private int _framePassed;

    public RedDotManager()
    {
        _framePassed = 0;
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
        _dirtyRedDotUnitSet = new HashSet<RedDotUnit>();
    }

    /// <summary>
    /// 执行所有红点名运算单元运算
    /// Note:
    /// 进入游戏后获取完相关数据后触发一次,确保第一次运算结果缓存
    /// </summary>
    public void DoAllRedDotUnitCalculate()
    {
        var redDotUnitInfoMap = RedDotModel.Singleton.GetRedDotUnitInfoMap();
        // 先算带ID单元，再算普通/聚合单元，保证聚合读取到最新子单元快照。
        foreach(var redDotUnitInfo in redDotUnitInfoMap)
        {
            if(redDotUnitInfo.Value.SupportIdParameter)
                DoRedDotUnitCalculateWithId(redDotUnitInfo.Key);
        }
        foreach(var redDotUnitInfo in redDotUnitInfoMap)
        {
            if(!redDotUnitInfo.Value.SupportIdParameter)
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

    /// <summary>
    /// 触发带ID红点名刷新回调
    /// </summary>
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

    /// <summary>
    /// 批量触发红点名刷新回调
    /// </summary>
    private void TriggerRedDotNameUpdate(HashSet<string> affectedRedDotNames)
    {
        foreach (var redDotName in affectedRedDotNames)
        {
            TriggerRedDotNameUpdate(redDotName);
        }
    }

    /// <summary>
    /// 批量触发带ID红点名刷新回调
    /// </summary>
    private void TriggerRedDotNameUpdateWithId(HashSet<string> affectedRedDotNames)
    {
        foreach (var redDotName in affectedRedDotNames)
        {
            TriggerRedDotNameUpdateWithId(redDotName);
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

    /// <summary>
    /// 获取带ID红点名的结果数据
    /// </summary>
    public (int result, RedDotType redDotType) GetRedDotNameResultWithId(string redDotName)
    {
        (int result, RedDotType redDotType) redDotNameResult = (0, RedDotType.NONE);
        redDotNameResult.result = 0;
        var redDotUnitList = RedDotModel.Singleton.GetRedDotUnitsByName(redDotName);
        if (redDotUnitList != null)
        {
            var result = 0;
            var redDotType = RedDotType.NONE;
            var redDotInfo = RedDotModel.Singleton.GetRedDotInfoByName(redDotName);
            var id = redDotInfo != null && redDotInfo.IsIdBased ? redDotInfo.Id : 0;
            foreach (var redDotUnit in redDotUnitList)
            {
                var redDotUnitResult = 0;
                var redDotUnitInfo = RedDotModel.Singleton.GetRedDotUnitInfo(redDotUnit);
                if (redDotUnitInfo != null && redDotUnitInfo.SupportIdParameter)
                {
                    redDotUnitResult = RedDotModel.Singleton.GetRedDotUnitResult(redDotUnit, id);
                }
                else
                {
                    redDotUnitResult = RedDotModel.Singleton.GetRedDotUnitResult(redDotUnit);
                }
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
    /// 绑定指定红点名刷新回调并立即回放当前结果
    /// </summary>
    public void BindRedDotNameAndReplayCurrent(string redDotName, Action<string, int, RedDotType> refreshDelegate)
    {
        var redDotInfo = RedDotModel.Singleton.GetRedDotInfoByName(redDotName);
        if(redDotInfo == null)
        {
            Debug.LogError($"找不到红点名:{redDotName}红点信息,绑定刷新失败!");
            return;
        }
        if(!redDotInfo.Bind(refreshDelegate))
        {
            return;
        }
        if(redDotInfo.IsIdBased)
        {
            var redDotNameResult = GetRedDotNameResultWithId(redDotName);
            refreshDelegate.Invoke(redDotName, redDotNameResult.result, redDotNameResult.redDotType);
            return;
        }
        var currentRedDotNameResult = GetRedDotNameResult(redDotName);
        refreshDelegate.Invoke(redDotName, currentRedDotNameResult.result, currentRedDotNameResult.redDotType);
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
        if(!RedDotModel.Singleton.IsInitComplete)
        {
            return;
        }
        _framePassed++;
        if(_framePassed >= _dirtyUpdateIntervalFrame)
        {
            // 先处理带ID叶子单元，再处理非ID/聚合单元，确保聚合读取的是最新子结果。
            ProcessDirtyRedDotUnitWithId();
            ProcessDirtyRedDotUnit();
            _framePassed = 0;
        }
    }

    /// <summary>
    /// 标记指定红点运算类型脏
    /// </summary>
    /// <param name="redDotUnit"></param>
    public void MarkRedDotUnitDirty(RedDotUnit redDotUnit)
    {
        var visitedUnits = new HashSet<RedDotUnit>();
        MarkRedDotUnitDirtyRecursive(redDotUnit, visitedUnits);
    }

    /// <summary>
    /// 标记指定带ID红点运算单元脏，并向上游聚合单元传播
    /// </summary>
    public void MarkRedDotUnitDirty(RedDotUnit redDotUnit, int id)
    {
        var redDotUnitWithId = new RedDotUnitWithId(redDotUnit, id);
        if(!_dirtyRedDotUnitWithId.Contains(redDotUnitWithId))
             _dirtyRedDotUnitWithId.Add(redDotUnitWithId);
        var visitedUnits = new HashSet<RedDotUnit>();
        MarkDependentRedDotUnitDirty(redDotUnit, visitedUnits);
    }

    /// <summary>
    /// 递归标记红点运算单元脏，避免重复访问
    /// </summary>
    private void MarkRedDotUnitDirtyRecursive(RedDotUnit redDotUnit, HashSet<RedDotUnit> visitedUnits)
    {
        if (visitedUnits.Contains(redDotUnit))
        {
            return;
        }
        visitedUnits.Add(redDotUnit);
        if (!_dirtyRedDotUnitSet.Contains(redDotUnit))
        {
            _dirtyRedDotUnitSet.Add(redDotUnit);
        }
        MarkDependentRedDotUnitDirty(redDotUnit, visitedUnits);
    }

    /// <summary>
    /// 按依赖关系标记上游聚合单元脏
    /// </summary>
    private void MarkDependentRedDotUnitDirty(RedDotUnit redDotUnit, HashSet<RedDotUnit> visitedUnits)
    {
        var dependentUnits = RedDotModel.Singleton.GetDependentRedDotUnits(redDotUnit);
        if (dependentUnits == null || dependentUnits.Count == 0)
        {
            return;
        }
        foreach (var dependentUnit in dependentUnits)
        {
            // 聚合单元递归标脏，支持多层单元组合。
            MarkRedDotUnitDirtyRecursive(dependentUnit, visitedUnits);
        }
    }

    /// <summary>
    /// 处理非ID红点运算单元的标脏与刷新
    /// </summary>
    private void ProcessDirtyRedDotUnit()
    {
        var affectedRedDotNames = CollectAffectedRedDotNames();
        _dirtyRedDotUnitSet.Clear();
        TriggerRedDotNameUpdate(affectedRedDotNames);
    }

    /// <summary>
    /// 收集非ID红点中受影响的红点名
    /// </summary>
    private HashSet<string> CollectAffectedRedDotNames()
    {
        var calculatedRedDotUnitResultChangeMap = new Dictionary<RedDotUnit, bool>();
        var affectedRedDotNames = new HashSet<string>();
        foreach(var dirtyRedDotUnit in _dirtyRedDotUnitSet)
        {
            var redDotNameList = RedDotModel.Singleton.GetRedDotNamesByUnit(dirtyRedDotUnit);
            if(redDotNameList == null || redDotNameList.Count == 0)
            {
                continue;
            }
            foreach (var redDotName in redDotNameList)
            {
                RecalculateRedDotNameUnits(redDotName, calculatedRedDotUnitResultChangeMap);
                affectedRedDotNames.Add(redDotName);
            }
        }
        return affectedRedDotNames;
    }

    /// <summary>
    /// 处理带ID红点运算单元的标脏与刷新
    /// </summary>
    private void ProcessDirtyRedDotUnitWithId()
    {
        var affectedRedDotNames = CollectAffectedRedDotNamesWithId();
        TriggerRedDotNameUpdateWithId(affectedRedDotNames);
        _dirtyRedDotUnitWithId.Clear();
    }

    /// <summary>
    /// 收集带ID红点中受影响的红点名
    /// </summary>
    private HashSet<string> CollectAffectedRedDotNamesWithId()
    {
        var affectedRedDotNames = new HashSet<string>();
        foreach (var dirtyRedDotUnitWithId in _dirtyRedDotUnitWithId)
        {
            var redDotUnit = dirtyRedDotUnitWithId.RedDotUnit;
            var id = dirtyRedDotUnitWithId.Id;
            var names = RedDotModel.Singleton.GetRedDotNameByUnitWithId(redDotUnit, id);
            if (names == null || names.Count == 0)
            {
                continue;
            }
            foreach (var nmWithId in names)
            {
                var nm = nmWithId.RedDotName;
                DoRedDotNameCalculateWithId(nm, id);
                affectedRedDotNames.Add(nm);
            }
        }
        return affectedRedDotNames;
    }

    /// <summary>
    /// 重算指定红点名关联的所有运算单元
    /// </summary>
    private void RecalculateRedDotNameUnits(string redDotName, Dictionary<RedDotUnit, bool> calculatedRedDotUnitResultChangeMap)
    {
        var redDotUnitList = RedDotModel.Singleton.GetRedDotUnitsByName(redDotName);
        if(redDotUnitList == null || redDotUnitList.Count == 0)
        {
            return;
        }
        foreach(var redDotUnit in redDotUnitList)
        {
            if(calculatedRedDotUnitResultChangeMap.ContainsKey(redDotUnit))
            {
                continue;
            }
            var resultChange = DoRedDotUnitCalculate(redDotUnit);
            calculatedRedDotUnitResultChangeMap.Add(redDotUnit, resultChange);
        }
    }

    /// <summary>
    /// 计算指定非ID红点运算单元并返回结果是否变化
    /// </summary>
    private bool DoRedDotUnitCalculate(RedDotUnit redDotUnit)
    {
        var preResult = RedDotModel.Singleton.GetRedDotUnitResult(redDotUnit);
        var redDotUnitInfo = RedDotModel.Singleton.GetRedDotUnitInfo(redDotUnit);
        var result = 0;
        if(redDotUnitInfo != null && redDotUnitInfo.CalculateMode == RedDotUnitCalculateMode.COMPOSITE)
        {
            result = DoCompositeRedDotUnitCalculate(redDotUnitInfo);
        }
        else
        {
            var redDotUnitFunc = RedDotModel.Singleton.GetRedDotUnitFunc(redDotUnit);
            if(redDotUnitFunc != null)
            {
                result = redDotUnitFunc();
            }
            else
            {
                Debug.LogError($"红点运算单元:{redDotUnit.ToString()}未绑定有效计算方法!");
            }
        }
        RedDotModel.Singleton.SetRedDotUnitResult(redDotUnit, result);
        return preResult != result;
    }

    /// <summary>
    /// 计算聚合红点运算单元结果
    /// </summary>
    private int DoCompositeRedDotUnitCalculate(RedDotUnitInfo redDotUnitInfo)
    {
        if (redDotUnitInfo.DependencyUnits == null || redDotUnitInfo.DependencyUnits.Count == 0)
        {
            return 0;
        }
        if (redDotUnitInfo.AggregateMode == RedDotUnitAggregateMode.ANY_POSITIVE)
        {
            foreach (var dependencyUnit in redDotUnitInfo.DependencyUnits)
            {
                var dependencyInfo = RedDotModel.Singleton.GetRedDotUnitInfo(dependencyUnit);
                if (dependencyInfo != null && dependencyInfo.SupportIdParameter)
                {
                    // 聚合静态单元时从红点系统内部ID缓存聚合，不依赖业务层角色列表。
                    var ids = RedDotModel.Singleton.GetRegisteredIdsByUnit(dependencyUnit);
                    foreach (var id in ids)
                    {
                        if (RedDotModel.Singleton.GetRedDotUnitResult(dependencyUnit, id) > 0)
                        {
                            return 1;
                        }
                    }
                    continue;
                }
                if (RedDotModel.Singleton.GetRedDotUnitResult(dependencyUnit) > 0)
                {
                    return 1;
                }
            }
            return 0;
        }
        var totalResult = 0;
        foreach (var dependencyUnit in redDotUnitInfo.DependencyUnits)
        {
            var dependencyInfo = RedDotModel.Singleton.GetRedDotUnitInfo(dependencyUnit);
            if (dependencyInfo != null && dependencyInfo.SupportIdParameter)
            {
                var ids = RedDotModel.Singleton.GetRegisteredIdsByUnit(dependencyUnit);
                foreach (var id in ids)
                {
                    totalResult += RedDotModel.Singleton.GetRedDotUnitResult(dependencyUnit, id);
                }
                continue;
            }
            totalResult += RedDotModel.Singleton.GetRedDotUnitResult(dependencyUnit);
        }
        return totalResult;
    }

    /// <summary>
    /// 按红点名逐个重算带ID红点运算单元缓存
    /// </summary>
    private void DoRedDotUnitCalculateWithId(RedDotUnit redDotUnit)
    {
        var reddotNameList = RedDotModel.Singleton.GetRedDotNamesByUnit(redDotUnit);
        if(reddotNameList == null || reddotNameList.Count == 0)
        {
            return ;
        }
        foreach(var redDotName in reddotNameList)
        {
            var redDotInfo = RedDotModel.Singleton.GetRedDotInfoByName(redDotName);
            if (redDotInfo == null)
            {
                continue;
            }
            var id = redDotInfo.IsIdBased ? redDotInfo.Id : 0;
            DoRedDotNameCalculateWithId(redDotName, id);
        }
        return ;
    }

    /// <summary>
    /// 计算带ID红点名结果并回写单元与红点名缓存
    /// </summary>
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
            var redDotUnitResult = 0;
            var redDotUnitInfo = RedDotModel.Singleton.GetRedDotUnitInfo(redDotUnit);
            if (redDotUnitInfo != null && redDotUnitInfo.SupportIdParameter)
            {
                if (redDotUnitInfo.RedDotUnitCalculateFuncWithId != null)
                {
                    redDotUnitResult = redDotUnitInfo.RedDotUnitCalculateFuncWithId(id);
                }
                RedDotModel.Singleton.SetRedDotUnitResult(redDotUnit, id, redDotUnitResult);
            }
            else
            {
                var func = RedDotModel.Singleton.GetRedDotUnitFunc(redDotUnit);
                if (func != null)
                {
                    redDotUnitResult = func();
                }
                RedDotModel.Singleton.SetRedDotUnitResult(redDotUnit, redDotUnitResult);
            }
            totalResult += redDotUnitResult;
        }
        var preResult = RedDotModel.Singleton.GetRedDotNameResult(redDotName);
        RedDotModel.Singleton.SetRedDotNameResult(redDotName, totalResult);
        return preResult != totalResult;
    }
    
    public bool TryParseRedDotNameWithId(string redDotName, out int id)
    {
        id = 0;
        var redDotInfo = RedDotModel.Singleton.GetRedDotInfoByName(redDotName);
        if (redDotInfo == null || !redDotInfo.IsIdBased)
        {
            return false;
        }
        id = redDotInfo.IsIdBased ? redDotInfo.Id : 0;
        return true;
    }
}
