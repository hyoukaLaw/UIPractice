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
        RedDotManager.Singleton.DoAllRedDotUnitCaculate();
        
        UIManager.Instance.ShowPanel(UIPanelType.Main);
        
    }

    // Update is called once per frame
    void Update()
    {
        RedDotManager.Singleton.Update();
    }
}
