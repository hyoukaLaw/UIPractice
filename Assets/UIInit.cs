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
        UIManager.Instance.ShowPanel(UIPanelType.Main);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
