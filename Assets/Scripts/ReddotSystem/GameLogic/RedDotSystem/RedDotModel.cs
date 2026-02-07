/*
 * Description:             RedDotModel.cs
 * Author:                  TONYTANG
 * Create Date:             2022/08/14
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDotNameWithId : IEquatable<RedDotNameWithId>
{
    public string RedDotName;
    public int Id;

    public RedDotNameWithId(string redDotName, int id)
    {
        RedDotName = redDotName;
        Id = id;
    }

    public bool Equals(RedDotNameWithId other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return RedDotName == other.RedDotName && Id == other.Id;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((RedDotNameWithId)obj);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(RedDotName, Id);
    }
}

public class RedDotUnitWithId: IEquatable<RedDotUnitWithId>
{
    public RedDotUnit RedDotUnit;
    public int Id;

    public RedDotUnitWithId(RedDotUnit redDotUnit, int id)
    {
        RedDotUnit = redDotUnit;
        Id = id;
    }

    public bool Equals(RedDotUnitWithId other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return RedDotUnit == other.RedDotUnit && Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((RedDotUnitWithId)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)RedDotUnit, Id);
    }
}
/// <summary>
/// RedDotModel.cs
/// 红点数据单例类
/// </summary>
public class RedDotModel : SingletonTemplate<RedDotModel>
{
    /// <summary>
    /// 红点运算单元信息Map<红点运算单元名, 红点运算单元信息>
    /// </summary>
    private Dictionary<RedDotUnit, RedDotUnitInfo> _redDotUnitInfoMap;

    /// <summary>
    /// 红点运算单元结果值Map<红点运算单元, 红点运算单元结果值>
    /// </summary>
    private Dictionary<RedDotUnit, int> _redDotUnitResultMap;

    /// <summary>
    /// 带Id红点运算单元结果值Map<红点运算单元+Id, 红点运算单元结果值>
    /// </summary>
    private Dictionary<RedDotUnitWithId, int> _redDotUnitResultWithIdMap;

    /// <summary>
    /// 红点名和红点信息Map<红点名, 红点信息>
    /// </summary>
    private Dictionary<string, RedDotInfo> _redDotInfoMap;

    /// <summary>
    /// 红点单元和红点名列表Map
    /// </summary>
    private Dictionary<RedDotUnit, List<string>> _redDotUnitNameMap;

    /// <summary>
    /// 红点名结果值Map<红点名, 红点名结果值>
    /// </summary>
    private Dictionary<string, int> _redDotNameResultMap;

    /// <summary>
    /// 带Id红点运算单元和红点名列表Map
    /// </summary>
    private Dictionary<RedDotUnitWithId, List<RedDotNameWithId>> _redDotUnitWithIdMap = new();

    /// <summary>
    /// 红点运算单元依赖反向索引Map<Key:被依赖单元, Value:依赖它的聚合单元列表>
    /// </summary>
    private Dictionary<RedDotUnit, List<RedDotUnit>> _redDotUnitDependencyMap = new();

    public Trie RedDotTrie
    {
        get;
        private set;
    }

    /// <summary>
    /// 初始化是否完成
    /// </summary>
    public bool IsInitCompelte
    {
        get;
        private set;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public RedDotModel()
    {
        _redDotUnitInfoMap = new Dictionary<RedDotUnit, RedDotUnitInfo>();
        _redDotUnitResultMap = new Dictionary<RedDotUnit, int>();
        _redDotUnitResultWithIdMap = new Dictionary<RedDotUnitWithId, int>();
        _redDotInfoMap = new Dictionary<string, RedDotInfo>();
        _redDotUnitNameMap = new Dictionary<RedDotUnit, List<string>>();
        _redDotNameResultMap = new Dictionary<string, int>();
        IsInitCompelte = false;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        if (IsInitCompelte)
        {
            Debug.LogError($"请勿重复初始化!");
            return;
        }
        // 优先初始化红点单元，现在改成通过红点名正向配置红点单元组成，而非反向红点单元定义影响红点名组成
        InitRedDotUnitInfo();
        InitRedDotInfo();
        InitRedDotTree();
        // InitRedDotUnitNameMap必须在InitRedDotInfo之后调用，因为用到了前面的数据
        UpdateRedDotUnitNameMap();
        IsInitCompelte = true;
    }

    /// <summary>
    /// 初始化红点运算单元信息
    /// </summary>
    private void InitRedDotUnitInfo()
    {
        AddRedDotUnitInfo(RedDotUnit.NEW_FUNC1, "动态新功能1解锁", RedDotUtilities.CaculateNewFunc1, RedDotType.NEW);
        AddRedDotUnitInfo(RedDotUnit.NEW_FUNC2, "动态新功能2解锁", RedDotUtilities.CaculateNewFunc2, RedDotType.NEW);
        AddRedDotUnitInfo(RedDotUnit.NEW_ITEM_NUM, "新道具数", RedDotUtilities.CaculateNewItemNum, RedDotType.NUMBER);
        AddRedDotUnitInfo(RedDotUnit.NEW_RESOURCE_NUM, "新资源数", RedDotUtilities.CaculateNewResourceNum, RedDotType.NUMBER);
        AddRedDotUnitInfo(RedDotUnit.NEW_EQUIP_NUM, "新装备数", RedDotUtilities.CaculateNewEquipNum, RedDotType.NUMBER);
        AddRedDotUnitInfo(RedDotUnit.NEW_PUBLIC_MAIL_NUM, "新公共邮件数", RedDotUtilities.CaculateNewPublicMailNum, RedDotType.NUMBER);
        AddRedDotUnitInfo(RedDotUnit.NEW_BATTLE_MAIL_NUM, "新战斗邮件数", RedDotUtilities.CaculateNewBattleMailNum, RedDotType.NUMBER);
        AddRedDotUnitInfo(RedDotUnit.NEW_OTHER_MAIL_NUM, "新其他邮件数", RedDotUtilities.CaculateNewOtherMailNum, RedDotType.NUMBER);
        AddRedDotUnitInfo(RedDotUnit.PUBLIC_MAIL_REWARD_NUM, "公共邮件可领奖数", RedDotUtilities.CaculateNewPublicMailRewardNum, RedDotType.NUMBER);
        AddRedDotUnitInfo(RedDotUnit.BATTLE_MAIL_REWARD_NUM, "战斗邮件可领奖数", RedDotUtilities.CaculateNewBattleMailRewardNum, RedDotType.NUMBER);
        AddRedDotUnitInfo(RedDotUnit.WEARABLE_EQUIP_NUM, "可穿戴装备数", RedDotUtilities.CaculateWearableEquipNum, RedDotType.NUMBER);
        AddRedDotUnitInfo(RedDotUnit.UPGRADEABLE_EQUIP_NUM, "可升级装备数", RedDotUtilities.CaculateUpgradeableEquipNum, RedDotType.NUMBER);
        AddRedDotUnitInfoWithId(RedDotUnit.CHARACTER_STORY_NEW, "人物新故事", RedDotUtilities.CalculateCharacterStoryNew, RedDotType.NEW);
        AddRedDotUnitInfoWithId(RedDotUnit.CHARACTER_CG_NEW, "人物新Cg", RedDotUtilities.CalculateCharacterCgNew, RedDotType.NEW);
        AddCompositeRedDotUnitInfo(RedDotUnit.MAIN_UI_CHARACTER_NEW, "新人物", new List<RedDotUnit>() { RedDotUnit.CHARACTER_STORY_NEW, RedDotUnit.CHARACTER_CG_NEW }, RedDotType.NEW, RedDotUnitAggregateMode.ANY_POSITIVE);
        // 可以强行给MAIN_CHARACTER_NEW配置不带int参数的版本，但本质上就不是理想的聚合方式了，除非配置每个红点名字模板对应的ID数量，可以实现接洽
        //AddRedDotUnitInfoWithId(RedDotUnit.CHARACTER_NEW, "人物有新东西", RedDotUtilities.CalculateCharacterNew, RedDotType.NEW);
    }

    /// <summary>
    /// 初始化红点信息
    /// </summary>
    private void InitRedDotInfo()
    {
        /// Note:
        /// 穷举的好处是足够灵活
        /// 缺点是删除最里层红点运算单元需要把外层所有影响到的红点名相关红点运算单元配置删除
        /// 调用AddRedDotInfo添加游戏所有静态红点信息
        InitMainUIRedDotInfo();
        InitBackpackUIRedDotInfo();
        InitMailUIRedDotInfo();
        InitEquipUIRedDotInfo();
    }

    /// <summary>
    /// 初始化主界面红点信息
    /// </summary>
    private void InitMainUIRedDotInfo()
    {
        RedDotInfo redDotInfo;
        redDotInfo = AddRedDotInfo(RedDotNames.MAIN_UI_NEW_FUNC1, "主界面新功能1红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_FUNC1);

        redDotInfo = AddRedDotInfo(RedDotNames.MAIN_UI_NEW_FUNC2, "主界面新功能2红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_FUNC2);

        redDotInfo = AddRedDotInfo(RedDotNames.MAIN_UI_MENU, "主界面菜单红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_ITEM_NUM);
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_RESOURCE_NUM);
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_EQUIP_NUM);

        redDotInfo = AddRedDotInfo(RedDotNames.MAIN_UI_MAIL, "主界面邮件红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_PUBLIC_MAIL_NUM);
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_BATTLE_MAIL_NUM);
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_OTHER_MAIL_NUM);
        redDotInfo.AddRedDotUnit(RedDotUnit.PUBLIC_MAIL_REWARD_NUM);

        redDotInfo = AddRedDotInfo(RedDotNames.MAIN_UI_MENU_EQUIP, "主界面菜单装备红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.WEARABLE_EQUIP_NUM);
        redDotInfo.AddRedDotUnit(RedDotUnit.UPGRADEABLE_EQUIP_NUM);

        redDotInfo = AddRedDotInfo(RedDotNames.MAIN_UI_MENU_BACKPACK, "主界面菜单背包红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_ITEM_NUM);
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_RESOURCE_NUM);

        redDotInfo = AddRedDotInfo(RedDotNames.MAIN_UI_CHARACTER, "主界面人物红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.MAIN_UI_CHARACTER_NEW);
    }

    /// <summary>
    /// 初始化背包界面红点信息
    /// </summary>
    private void InitBackpackUIRedDotInfo()
    {
        RedDotInfo redDotInfo;
        redDotInfo = AddRedDotInfo(RedDotNames.BACKPACK_UI_ITEM_TAG, "背包界面道具页签红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_ITEM_NUM);

        redDotInfo = AddRedDotInfo(RedDotNames.BACKPACK_UI_RESOURCE_TAG, "背包界面资源页签红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_RESOURCE_NUM);

        redDotInfo = AddRedDotInfo(RedDotNames.BACKPACK_UI_EQUIP_TAG, "背包界面装备页签红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_EQUIP_NUM);
    }

    /// <summary>
    /// 初始化邮件界面红点信息
    /// </summary>
    private void InitMailUIRedDotInfo() // 这里绑定了红点名和红点的逻辑
    {
        RedDotInfo redDotInfo;
        redDotInfo = AddRedDotInfo(RedDotNames.MAIL_UI_PUBLIC_MAIL, "邮件界面公共邮件红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_PUBLIC_MAIL_NUM);
        redDotInfo.AddRedDotUnit(RedDotUnit.PUBLIC_MAIL_REWARD_NUM);

        redDotInfo = AddRedDotInfo(RedDotNames.MAIL_UI_BATTLE_MAIL, "邮件界面战斗邮件红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_BATTLE_MAIL_NUM);
        redDotInfo.AddRedDotUnit(RedDotUnit.BATTLE_MAIL_REWARD_NUM);

        redDotInfo = AddRedDotInfo(RedDotNames.MAIL_UI_OTHER_MAIL, "邮件界面其他邮件红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.NEW_OTHER_MAIL_NUM);
    }

    /// <summary>
    /// 初始化装备界面红点信息
    /// </summary>
    private void InitEquipUIRedDotInfo()
    {
        RedDotInfo redDotInfo;
        redDotInfo = AddRedDotInfo(RedDotNames.EQUIP_UI_WEARABLE, "装备界面可穿戴红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.WEARABLE_EQUIP_NUM);

        redDotInfo = AddRedDotInfo(RedDotNames.EQUIP_UI_UPGRADABLE, "装备界面可升级红点");
        redDotInfo.AddRedDotUnit(RedDotUnit.UPGRADEABLE_EQUIP_NUM);
    }
    
    

    /// <summary>
    /// 添加红点信息
    /// </summary>
    /// <param name="redDotName"></param>
    /// <param name="redDotDes"></param>
    public RedDotInfo AddRedDotInfo(string redDotName, string redDotDes)
    {
        if (_redDotInfoMap.ContainsKey(redDotName))
        {
            Debug.LogError($"重复添加红点名:{redDotName}信息，添加失败!");
            return null;
        }
        var redDotInfo = new RedDotInfo(redDotName, redDotDes);
        _redDotInfoMap.Add(redDotName, redDotInfo);
        return redDotInfo;
    }

    /// <summary>
    /// 构建红点前缀树
    /// </summary>
    private void InitRedDotTree()
    {
        RedDotTrie = new Trie();
        foreach (var redDotInfo in _redDotInfoMap)
        {
            RedDotTrie.AddWord(redDotInfo.Key);
        }
    }

    /// <summary>
    /// 根据_redDotInfoMap反向构建_redDotUnitNameMap
    /// </summary>
    private void UpdateRedDotUnitNameMap()
    {
        _redDotUnitNameMap.Clear();
        foreach (var redDotInfo in _redDotInfoMap)
        {
            foreach (var redDotUnit in redDotInfo.Value.RedDotUnitList)
            {
                if (!_redDotUnitNameMap.ContainsKey(redDotUnit))
                {
                    _redDotUnitNameMap.Add(redDotUnit, new List<string>());
                }
                if (!_redDotUnitNameMap[redDotUnit].Contains(redDotInfo.Key))
                {
                    _redDotUnitNameMap[redDotUnit].Add(redDotInfo.Value.RedDotName);
                }
            }
        }
    }

    /// <summary>
    /// 添加红点运算单元信息
    /// </summary>
    /// <param name="redDotUnit"></param>
    /// <param name="redDotUnitDes"></param>
    /// <param name="caculateFunc"></param>
    /// <param name="redDotType"></param>
    /// <returns></returns>
    private RedDotUnitInfo AddRedDotUnitInfo(RedDotUnit redDotUnit, string redDotUnitDes, Func<int> caculateFunc, RedDotType redDotType = RedDotType.NUMBER)
    {
        RedDotUnitInfo redDotUnitInfo;
        if(_redDotUnitInfoMap.TryGetValue(redDotUnit, out redDotUnitInfo))
        {
            Debug.LogError($"已添加红点运算单元:{redDotUnit.ToString()}的红点运算单元信息,请勿重复添加,添加失败!");
            return redDotUnitInfo;
        }
        redDotUnitInfo = new RedDotUnitInfo(redDotUnit, redDotUnitDes, caculateFunc, redDotType);
        _redDotUnitInfoMap.Add(redDotUnit, redDotUnitInfo);
        return redDotUnitInfo;
    }

    /// <summary>
    /// 添加带Id红点运算单元信息
    /// </summary>
    private RedDotUnitInfo AddRedDotUnitInfoWithId(RedDotUnit redDotUnit, string redDotUnitDes, Func<int, int> caculateFunc, RedDotType redDotType = RedDotType.NUMBER)
    {
        RedDotUnitInfo redDotUnitInfo;
        if(_redDotUnitInfoMap.TryGetValue(redDotUnit, out redDotUnitInfo))
        {
            Debug.LogError($"已添加红点运算单元:{redDotUnit.ToString()}的红点运算单元信息,请勿重复添加,添加失败!");
            return redDotUnitInfo;
        }
        redDotUnitInfo = new RedDotUnitInfo(redDotUnit, redDotUnitDes, caculateFunc, redDotType);
        _redDotUnitInfoMap.Add(redDotUnit, redDotUnitInfo);
        return redDotUnitInfo;
    }

    /// <summary>
    /// 添加聚合红点运算单元信息
    /// </summary>
    private RedDotUnitInfo AddCompositeRedDotUnitInfo(RedDotUnit redDotUnit, string redDotUnitDes, List<RedDotUnit> dependencyUnits, RedDotType redDotType, RedDotUnitAggregateMode redDotUnitAggregateMode)
    {
        RedDotUnitInfo redDotUnitInfo;
        if (_redDotUnitInfoMap.TryGetValue(redDotUnit, out redDotUnitInfo))
        {
            Debug.LogError($"已添加红点运算单元:{redDotUnit.ToString()}的红点运算单元信息,请勿重复添加,添加失败!");
            return redDotUnitInfo;
        }
        redDotUnitInfo = new RedDotUnitInfo(redDotUnit, redDotUnitDes, dependencyUnits, redDotType, redDotUnitAggregateMode);
        _redDotUnitInfoMap.Add(redDotUnit, redDotUnitInfo);
        // 构建单元依赖反向索引，用于脏标记自动向上游聚合单元传播。
        foreach (var dependencyUnit in dependencyUnits)
        {
            if (!_redDotUnitDependencyMap.ContainsKey(dependencyUnit))
            {
                _redDotUnitDependencyMap.Add(dependencyUnit, new List<RedDotUnit>());
            }
            if (!_redDotUnitDependencyMap[dependencyUnit].Contains(redDotUnit))
            {
                _redDotUnitDependencyMap[dependencyUnit].Add(redDotUnit);
            }
        }
        return redDotUnitInfo;
    }

    /// <summary>
    /// 获取红点名和红点信息Map<红点名, 红点信息>
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, RedDotInfo> GetRedDotInfoMap()
    {
        return _redDotInfoMap;
    }

    /// <summary>
    /// 获取红点运算单元信息Map<红点运算单元门, 红点运算单元信息>
    /// </summary>
    /// <returns></returns>
    public Dictionary<RedDotUnit, RedDotUnitInfo> GetRedDotUnitInfoMap()
    {
        return _redDotUnitInfoMap;
    }

    /// <summary>
    /// 获取指定红点名的红点信息
    /// </summary>
    /// <param name="redDotName"></param>
    /// <returns></returns>
    public RedDotInfo GetRedDotInfoByName(string redDotName)
    {
        RedDotInfo redDotInfo;
        if(!_redDotInfoMap.TryGetValue(redDotName, out redDotInfo))
        {
            Debug.LogError($"找不到红点名:{redDotName}的红点信息!");
        }
        return redDotInfo;
    }

    /// <summary>
    /// 获取指定红点运算单元的红点运算单元信息
    /// </summary>
    /// <param name="redDotUnit"></param>
    /// <returns></returns>
    public RedDotUnitInfo GetRedDotUnitInfo(RedDotUnit redDotUnit)
    {
        RedDotUnitInfo redDotUnitInfo;
        if(!_redDotUnitInfoMap.TryGetValue(redDotUnit, out redDotUnitInfo))
        {
            Debug.LogError($"找不到红点运算单元:{redDotUnit.ToString()}的信息!");
        }
        return redDotUnitInfo;
    }

    /// <summary>
    /// 获取指定红点运算单元的计算委托
    /// </summary>
    /// <param name="redDotUnit"></param>
    /// <returns></returns>
    public Func<int> GetRedDotUnitFunc(RedDotUnit redDotUnit)
    {
        RedDotUnitInfo redDotUnitInfo = GetRedDotUnitInfo(redDotUnit);
        return redDotUnitInfo?.RedDotUnitCalculateFunc;
    }

    /// <summary>
    /// 获取指定红点运算单元的红点类型
    /// </summary>
    /// <param name="redDotUnit"></param>
    /// <returns></returns>
    public RedDotType GetRedDotUnitRedType(RedDotUnit redDotUnit)
    {
        RedDotUnitInfo redDotUnitInfo = GetRedDotUnitInfo(redDotUnit);
        if(redDotUnitInfo != null)
        {
            return redDotUnitInfo.RedDotType;
        }
        Debug.LogError($"获取红点运算单元:{redDotUnit}的红点类型失败!");
        return RedDotType.NONE;
    }

    /// <summary>
    /// 获取指定红点名的所有红点运算单元列表
    /// </summary>
    /// <param name="redDotName"></param>
    /// <returns></returns>
    public List<RedDotUnit> GetRedDotUnitsByName(string redDotName)
    {
        var redDotInfo = GetRedDotInfoByName(redDotName);
        return redDotInfo != null ? redDotInfo.RedDotUnitList : null;
    }

    /// <summary>
    /// 获取指定红点运算单元影响的所有红点名列表
    /// </summary>
    /// <param name="redDotUnit"></param>
    /// <returns></returns>
    public List<string> GetRedDotNamesByUnit(RedDotUnit redDotUnit)
    {
        List<string> redDotNames;
        if(!_redDotUnitNameMap.TryGetValue(redDotUnit, out redDotNames))
        {
            Debug.LogError($"找不到红点运算单元:{redDotUnit.ToString()}的影响红点名信息，获取失败！");
        }
        return redDotNames;
    }

    /// <summary>
    /// 获取指定带Id红点运算单元影响的所有红点名列表
    /// </summary>
    public List<RedDotNameWithId> GetRedDotNameByUnitWithId(RedDotUnit redDotUnit, int id)
    {
        List<RedDotNameWithId> results;
        if (!_redDotUnitWithIdMap.TryGetValue(new RedDotUnitWithId(redDotUnit, id), out results))
        {
            Debug.LogError($"找不到红点运算单元:{redDotUnit.ToString()}的影响红点名信息，获取失败！");
        }
        return results;
    }

    /// <summary>
    /// 获取指定红点运算单元的上游依赖聚合单元列表
    /// </summary>
    public List<RedDotUnit> GetDependentRedDotUnits(RedDotUnit redDotUnit)
    {
        List<RedDotUnit> dependentUnits;
        if (!_redDotUnitDependencyMap.TryGetValue(redDotUnit, out dependentUnits))
        {
            return null;
        }
        return dependentUnits;
    }

    /// <summary>
    /// 获取指定带Id红点运算单元已注册的Id集合
    /// </summary>
    public HashSet<int> GetRegisteredIdsByUnit(RedDotUnit redDotUnit)
    {
        var ids = new HashSet<int>();
        // 通过已注册的动态红点映射提取可用ID，避免聚合单元读取业务层角色数据。
        foreach (var redDotUnitWithId in _redDotUnitWithIdMap)
        {
            if (redDotUnitWithId.Key.RedDotUnit == redDotUnit)
            {
                ids.Add(redDotUnitWithId.Key.Id);
            }
        }
        return ids;
    }

    /// <summary>
    /// 获取指定红点运算单元的运算结果
    /// </summary>
    /// <param name="redDotUnit"></param>
    /// <returns></returns>
    public int GetRedDotUnitResult(RedDotUnit redDotUnit)
    {
        int result = 0;
        if(!_redDotUnitResultMap.TryGetValue(redDotUnit, out result))
        {
        }
        return result;
    }

    /// <summary>
    /// 获取指定带Id红点运算单元的运算结果
    /// </summary>
    public int GetRedDotUnitResult(RedDotUnit redDotUnit, int id)
    {
        int result = 0;
        if(!_redDotUnitResultWithIdMap.TryGetValue(new RedDotUnitWithId(redDotUnit, id), out result))
        {
        }
        return result;
    }

    /// <summary>
    /// 设置指定红点运算单元的运算结果
    /// </summary>
    /// <param name="redDotUnit"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool SetRedDotUnitResult(RedDotUnit redDotUnit, int result)
    {
        if(!_redDotUnitResultMap.ContainsKey(redDotUnit))
        {
            _redDotUnitResultMap.Add(redDotUnit, result);
            return true;
        }
        _redDotUnitResultMap[redDotUnit] = result;
        return true;
    }

    /// <summary>
    /// 设置指定带Id红点运算单元的运算结果
    /// </summary>
    public bool SetRedDotUnitResult(RedDotUnit redDotUnit, int id, int result)
    {
        var redDotUnitWithId = new RedDotUnitWithId(redDotUnit, id);
        if(!_redDotUnitResultWithIdMap.ContainsKey(redDotUnitWithId))
        {
            _redDotUnitResultWithIdMap.Add(redDotUnitWithId, result);
            return true;
        }
        _redDotUnitResultWithIdMap[redDotUnitWithId] = result;
        return true;
    }

    /// <summary>
    /// 移除指定带Id红点运算单元的运算结果
    /// </summary>
    public bool RemoveRedDotUnitResult(RedDotUnit redDotUnit, int id)
    {
        return _redDotUnitResultWithIdMap.Remove(new RedDotUnitWithId(redDotUnit, id));
    }

    /// <summary>
    /// 注册带Id动态红点信息
    /// </summary>
    public RedDotInfo RegisterDynamicRedDot(int id, string redDotName, string redDotDes, RedDotUnit redDotUnit)
    {
        if (_redDotInfoMap.ContainsKey(redDotName))
        {
            Debug.LogWarning($"红点名:{redDotName}已存在，跳过注册!");
            return _redDotInfoMap[redDotName];
        }
        var redDotInfo = new RedDotInfo(redDotName, redDotDes, id);
        redDotInfo.AddRedDotUnit(redDotUnit);
        _redDotInfoMap.Add(redDotName, redDotInfo);
        RedDotTrie.AddWord(redDotName);
        if (!_redDotUnitNameMap.ContainsKey(redDotUnit))
        {
            _redDotUnitNameMap.Add(redDotUnit, new List<string>());
        }
        _redDotUnitNameMap[redDotUnit].Add(redDotName);
        if (!_redDotUnitWithIdMap.ContainsKey(new RedDotUnitWithId(redDotUnit, id)))
        {
            _redDotUnitWithIdMap.Add(new RedDotUnitWithId(redDotUnit, id), new List<RedDotNameWithId>());
        }
        _redDotUnitWithIdMap[new RedDotUnitWithId(redDotUnit, id)].Add(new RedDotNameWithId(redDotName, id));
        return redDotInfo;
    }

    /// <summary>
    /// 注册带Id动态红点信息
    /// </summary>
    public RedDotInfo RegisterDynamicRedDot(int id, string redDotName, string redDotDes, List<RedDotUnit> redDotUnitList)
    {
        if (_redDotInfoMap.ContainsKey(redDotName))
        {
            Debug.LogWarning($"红点名:{redDotName}已存在，跳过注册!");
            return _redDotInfoMap[redDotName];
        }
        var redDotInfo = new RedDotInfo(redDotName, redDotDes, id);
        _redDotInfoMap.Add(redDotName, redDotInfo);
        RedDotTrie.AddWord(redDotName);
        foreach (var redDotUnit in redDotUnitList)
        {
            redDotInfo.AddRedDotUnit(redDotUnit);
            if (!_redDotUnitNameMap.ContainsKey(redDotUnit))
            {
                _redDotUnitNameMap.Add(redDotUnit, new List<string>());
            }
            _redDotUnitNameMap[redDotUnit].Add(redDotName);
            if (!_redDotUnitWithIdMap.ContainsKey(new RedDotUnitWithId(redDotUnit, id)))
            {
                _redDotUnitWithIdMap.Add(new RedDotUnitWithId(redDotUnit, id), new List<RedDotNameWithId>());
            }
            _redDotUnitWithIdMap[new RedDotUnitWithId(redDotUnit, id)].Add(new RedDotNameWithId(redDotName, id));
        }

        return redDotInfo;
    }

    /// <summary>
    /// 注销带Id动态红点信息
    /// </summary>
    public void UnregisterDynamicRedDot(string redDotName, int id)
    {
        if (!_redDotInfoMap.ContainsKey(redDotName))
        {
            Debug.LogWarning($"红点名:{redDotName}不存在，跳过注销!");
            return;
        }
        var redDotInfo = _redDotInfoMap[redDotName];
        foreach (var redDotUnit in redDotInfo.RedDotUnitList)
        {
            if (_redDotUnitNameMap.ContainsKey(redDotUnit))
            {
                _redDotUnitNameMap[redDotUnit].Remove(redDotName);
            }

            if (_redDotUnitWithIdMap.ContainsKey(new RedDotUnitWithId(redDotUnit, id)))
            {
                var redDotUnitWithId = new RedDotUnitWithId(redDotUnit, id);
                _redDotUnitWithIdMap[redDotUnitWithId].Remove(new RedDotNameWithId(redDotName, id));
                if (_redDotUnitWithIdMap[redDotUnitWithId].Count == 0)
                {
                    _redDotUnitWithIdMap.Remove(redDotUnitWithId);
                }
            }
            RemoveRedDotUnitResult(redDotUnit, id);
        }
        _redDotNameResultMap.Remove(redDotName);
        RedDotTrie.RemoveWord(redDotName);
        _redDotInfoMap.Remove(redDotName);
    }

    /// <summary>
    /// 获取指定红点名缓存结果
    /// </summary>
    public int GetRedDotNameResult(string redDotName)
    {
        int result;
        if (_redDotNameResultMap.TryGetValue(redDotName, out result))
        {
            return result;
        }
        var redDotUnitList = GetRedDotUnitsByName(redDotName);
        if (redDotUnitList == null)
        {
            return 0;
        }
        result = 0;
        foreach (var redDotUnit in redDotUnitList)
        {
            result += GetRedDotUnitResult(redDotUnit);
        }
        return result;
    }

    /// <summary>
    /// 设置指定红点名缓存结果
    /// </summary>
    public bool SetRedDotNameResult(string redDotName, int result)
    {
        if (!_redDotNameResultMap.ContainsKey(redDotName))
        {
            _redDotNameResultMap.Add(redDotName, result);
            return true;
        }
        _redDotNameResultMap[redDotName] = result;
        return true;
    }
}
